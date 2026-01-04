using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class IssueTest : PageTest
{
    [SetUp]
    public async Task LoginSetup()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
    }

    [Test]
    public async Task TC_ISSUE_001_Repro_BrokenImages()
    {
        // Login as problem_user to trigger UI issues
        await Page.Locator("[data-test=\"username\"]").FillAsync("problem_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // Requirement: Ensure images load correctly
        var image = Page.Locator(".inventory_item_img img").First;
        var imageSrc = await image.GetAttributeAsync("src");

        // Assert

        // This will FAIL because the actual src contains "sl-404.168ba304.jpg"
        Assert.That(imageSrc, Does.Not.Contain("sl-404"),
            "BUG REPRO: Product image failed to load correctly for problem_user.");
    }

    [Test]
    public async Task TC_ISSUE_002_Repro_SortingFailure()
    {
        await Page.Locator("[data-test=\"username\"]").FillAsync("problem_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // Requirement: Sort Name (Z to A)
        await Page.Locator("[data-test=\"product-sort-container\"]").SelectOptionAsync("za");

        var firstName = await Page.Locator("[data-test=\"inventory-item-name\"]").First.InnerTextAsync();

        // Assert

        // This will FAIL because sorting doesn't function correctly for this user
        Assert.That(firstName, Is.EqualTo("Test.allTheThings() T-Shirt (Red)"),
            "BUG REPRO: Sorting Name (Z to A) did not update the UI.");
    }

    [Test]
    public async Task TC_ISSUE_003_Repro_CartRemoveFailure()
    {
        await Page.Locator("[data-test=\"username\"]").FillAsync("problem_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // Add item then try to remove it
        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Page.Locator("[data-test=\"remove-sauce-labs-backpack\"]").ClickAsync();

        // Assert

        // This will FAIL because the remove action is broken for problem_user
        var cartBadge = Page.Locator("[data-test=\"shopping-cart-badge\"]");
        await Expect(cartBadge).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task TC_ISSUE_004_Repro_LastNameInputBug()
    {
        await Page.Locator("[data-test=\"username\"]").FillAsync("problem_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Page.GotoAsync("https://www.saucedemo.com/checkout-step-one.html");

        // Input data
        await Page.Locator("[data-test=\"firstName\"]").FillAsync("John");
        await Page.Locator("[data-test=\"lastName\"]").FillAsync("Doe");

        // Assert

        // This will FAIL because the field only changes the first name or ignores input
        var lastNameValue = await Page.Locator("[data-test=\"lastName\"]").InputValueAsync();
        Assert.That(lastNameValue, Is.EqualTo("Doe"),
            "BUG REPRO: Last Name field failed to accept or retain the correct string.");
    }

    [Test]
    public async Task TC_ISSUE_005_Repro_EmptyCartCheckout()
    {
        // Steps 1-6: Login
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // Steps 7-10: Navigate to Cart page with no items
        await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();

        // Step 11: Click Checkout
        await Page.Locator("[data-test=\"checkout\"]").ClickAsync();

        // Steps 12-15: Fill Info and Continue
        await Page.Locator("[data-test=\"firstName\"]").FillAsync("Test");
        await Page.Locator("[data-test=\"lastName\"]").FillAsync("User");
        await Page.Locator("[data-test=\"postalCode\"]").FillAsync("12345");
        await Page.Locator("[data-test=\"continue\"]").ClickAsync();

        // Step 16: Finish the order
        await Page.Locator("[data-test=\"finish\"]").ClickAsync();

        // Assert

        // BUG REPRO ASSERTION:
        // This will fail and the message will appear in your Test Explorer/Error Report.
        var completeHeader = Page.Locator("[data-test=\"complete-header\"]");

        // We use a standard NUnit assertion for the custom message to avoid Playwright option errors.
        var isHeaderVisible = await completeHeader.IsVisibleAsync();
        Assert.That(isHeaderVisible, Is.False,
            "BUG REPRO: System allowed a checkout to complete with an empty cart. " +
            "Users should be blocked from initiating checkout if no products are present.");
    }

    [Test]
    public async Task TC_ISSUE_006_Repro_LoginPerformanceGlitch()
    {
        await Page.Locator("[data-test=\"username\"]").FillAsync("performance_glitch_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");

        var startTime = DateTime.Now;
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        var duration = DateTime.Now - startTime;

        // Assert

        // Requirement: Pages must load within 3 seconds
        // This will FAIL because the glitch user takes ~10 seconds
        Assert.That(duration.TotalSeconds, Is.LessThan(3),
            $"BUG REPRO: Login took {duration.TotalSeconds} seconds, exceeding the 3s requirement.");
    }

    [TearDown]
    public async Task TearDown()
    {
        // 1. Identify File Info: Generate a timestamped filename based on the test name
        string screenshotName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        // 2. Dynamic Pathing: Navigate from bin execution folder to the Project Source tree
        string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo projectFolder = Directory.GetParent(assemblyPath).Parent.Parent.Parent;

        // 3. Routing: Sort evidence into 'Pass' or 'Fail' folders based on test result
        string statusFolder = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed ? "Pass" : "Fail";
        string evidencePath = Path.Combine(projectFolder.FullName, "TestEvidence", statusFolder);

        // 4. Persistence: Ensure directories exist and save the full-page screenshot
        if (!Directory.Exists(evidencePath)) Directory.CreateDirectory(evidencePath);
        string fullPath = Path.Combine(evidencePath, screenshotName);

        await Page.ScreenshotAsync(new() { Path = fullPath, FullPage = true });

        // 5. Integration: Link the screenshot to the NUnit/Visual Studio Test Results output
        TestContext.AddTestAttachment(fullPath, $"{statusFolder} Evidence");

        // 6. Cleanup: Gracefully close the browser context
        await Context.CloseAsync();
    }
}