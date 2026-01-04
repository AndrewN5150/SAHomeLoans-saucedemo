using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CriticalUiTests : PageTest
{
    // --- AUTHENTICATION TESTS ---

    [Test]
    public async Task AUTH001_ValidUser_RedirectsToInventory()
    {
        // 1. Setup: Navigate to the landing page
        await Page.GotoAsync("https://www.saucedemo.com/");

        // 2. Action: Perform login with valid 'standard_user' credentials
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // 3. Assert: Verify the user is redirected to the products inventory page
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task AUTH007_Logout_RedirectsToLoginPage()
    {
        // 1. Setup: Navigate and log in to reach the authenticated state
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // 2. Action: Open the sidebar menu and click the Logout link
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Page.Locator("[data-test=\"logout-sidebar-link\"]").ClickAsync();

        // 3. Assert: Verify the session is terminated and user is returned to the login page
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/");
    }

    // --- CART TESTS ---

    [Test]
    public async Task CART001_AddBackpack_UpdatesBadgeCount()
    {
        // 1. Setup: Log in to the application
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // 2. Action: Click the 'Add to cart' button for the backpack
        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();

        // 3. Assert: Verify the button text changes to 'Remove' and the cart badge increments to '1'
        await Expect(Page.Locator("[data-test=\"remove-sauce-labs-backpack\"]")).ToHaveTextAsync("Remove");
        await Expect(Page.Locator("[data-test=\"shopping-cart-badge\"]")).ToHaveTextAsync("1");
    }

    // --- CHECKOUT TESTS ---

    [Test]
    public async Task CHKT011_CHKT012_CompleteCheckout_DisplaysThankYouMessage()
    {
        // 1. Setup: Log in and navigate through the cart to the checkout details page
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();
        await Page.Locator("[data-test=\"checkout\"]").ClickAsync();

        // 2. Action: Provide shipping information and finalize the purchase
        await Page.Locator("[data-test=\"firstName\"]").FillAsync("Test");
        await Page.Locator("[data-test=\"lastName\"]").FillAsync("User");
        await Page.Locator("[data-test=\"postalCode\"]").FillAsync("12345");
        await Page.Locator("[data-test=\"continue\"]").ClickAsync();

        // REQ-CHKT-011: Submit the final order
        await Page.Locator("[data-test=\"finish\"]").ClickAsync();

        // 3. Assert (REQ-CHKT-012): Verify the order completion header and descriptive text are displayed
        await Expect(Page.Locator("[data-test=\"complete-header\"]")).ToHaveTextAsync("Thank you for your order!");
        await Expect(Page.Locator("[data-test=\"complete-text\"]")).ToHaveTextAsync("Your order has been dispatched, and will arrive just as fast as the pony can get there!");
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