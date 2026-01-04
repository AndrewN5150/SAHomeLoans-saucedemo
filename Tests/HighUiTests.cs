using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HighUiTests : PageTest
{
    private const string BaseUrl = "https://www.saucedemo.com/";

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.Locator("[data-test='username']").FillAsync("standard_user");
        await Page.Locator("[data-test='password']").FillAsync("secret_sauce");
        await Page.Locator("[data-test='login-button']").ClickAsync();
    }

    // --- INVENTORY TESTS ---

    [Test]
    public async Task PROD001_VerifyAllProducts_DisplaysCorrectCountAndInfo()
    {
        // 1. We hardcode the expected values to act as our "Source of Truth" against the UI
        var expectedProducts = new[]
        {
        new { Name = "Sauce Labs Backpack", Price = "$29.99", Desc = "carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection." },
        new { Name = "Sauce Labs Bike Light", Price = "$9.99", Desc = "A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included." },
        new { Name = "Sauce Labs Bolt T-Shirt", Price = "$15.99", Desc = "Get your testing superhero on with the Sauce Labs bolt T-shirt. From American Apparel, 100% ringspun combed cotton, heather gray with red bolt." },
        new { Name = "Sauce Labs Fleece Jacket", Price = "$49.99", Desc = "It's not every day that you come across a midweight quarter-zip fleece jacket capable of handling everything from a relaxing day outdoors to a busy day at the office." },
        new { Name = "Sauce Labs Onesie", Price = "$7.99", Desc = "Rib snap infant onesie for the junior automation engineer in development. Reinforced 3-snap bottom closure, two-needle hemmed sleeved and bottom won't unravel." },
        new { Name = "Test.allTheThings() T-Shirt (Red)", Price = "$15.99", Desc = "This classic Sauce Labs t-shirt is perfect to wear when cozying up to your keyboard to automate a few tests. Super-soft and comfy ringspun combed cotton." }
    };

        // 2. Ensure the catalog contains exactly 6 items before checking details
        var inventoryItems = Page.Locator("[data-test='inventory-item']");
        await Expect(inventoryItems).ToHaveCountAsync(6);

        // Assert

        // 3. We loop through our data source and find each specific item on the page to validate it
        foreach (var expected in expectedProducts)
        {
            // Find the specific inventory container that contains the expected product name
            var item = Page.Locator("[data-test='inventory-item']").Filter(new() { HasText = expected.Name });

            // Assert: Ensure the item card is actually visible to the user
            await Expect(item).ToBeVisibleAsync();

            // Assert: Verify the Name, Price, and Description match our "Source of Truth" exactly
            await Expect(item.Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(expected.Name);
            await Expect(item.Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(expected.Price);
            await Expect(item.Locator("[data-test='inventory-item-desc']")).ToHaveTextAsync(expected.Desc);

            // Assert: Verify the product image is present and visible within the item card
            await Expect(item.Locator(".inventory_item_img img")).ToBeVisibleAsync();
        }
    }

    [Test]
    public async Task PROD003_SortByNameAToZ_UpdatesListOrder()
    {
        // 1. Select the 'Name (A to Z)' option
        await Page.Locator("[data-test='product-sort-container']").SelectOptionAsync("az");

        // 2. Capture all product names from the page
        var names = await Page.Locator("[data-test='inventory-item-name']").AllInnerTextsAsync();

        // 3. Create a copy of the list and sort it mathematically in code
        var expectedOrder = names.OrderBy(x => x).ToList();

        // Assert

        // 4. Assert that the UI names match the expected Ascending order
        Assert.That(names, Is.EqualTo(expectedOrder), "The products were not sorted alphabetically from A to Z.");
    }

    [Test]
    public async Task PROD004_SortByNameZToA_UpdatesListOrder()
    {
        // 1. Select the 'Name (Z to A)' option
        await Page.Locator("[data-test='product-sort-container']").SelectOptionAsync("za");

        // 2. Capture all product names currently displayed on the page
        var names = await Page.Locator("[data-test='inventory-item-name']").AllInnerTextsAsync();

        // 3. Create the expected order by sorting the captured names descending in memory
        var expectedOrder = names.OrderByDescending(x => x).ToList();

        // Assert

        // 4. Assert that the UI matches the expected Z-A order
        Assert.That(names, Is.EqualTo(expectedOrder), "The products were not sorted alphabetically from Z to A.");
    }

    // --- INVENTORY SORTING TESTS ---

    [Test]
    public async Task PROD005_SortByPriceLowToHigh_UpdatesListOrder()
    {
        // 1. Action: Select the 'Price (low to high)' option from the dropdown
        await Page.Locator("[data-test='product-sort-container']").SelectOptionAsync("lohi");

        // 2. Data Capture: Scrape all price strings currently displayed on the inventory page
        var prices = await Page.Locator("[data-test='inventory-item-price']").AllInnerTextsAsync();

        // 3. Transformation: Convert currency strings to doubles using InvariantCulture 
        // This ensures decimal points (.) are parsed correctly regardless of local machine settings
        var numericPrices = prices
            .Select(p => double.Parse(p.Replace("$", ""), CultureInfo.InvariantCulture))
            .ToList();

        // Assert

        // 4. Assert: Verify the converted numerical list follows an ascending mathematical order
        Assert.That(numericPrices, Is.Ordered.Ascending);
    }

    [Test]
    public async Task PROD006_SortByPriceHighToLow_UpdatesListOrder()
    {
        // 1. Action: Select the 'Price (high to low)' option from the dropdown
        await Page.Locator("[data-test='product-sort-container']").SelectOptionAsync("hilo");

        // 2. Data Capture: Retrieve price text from all visible product cards
        var priceStrings = await Page.Locator("[data-test='inventory-item-price']").AllInnerTextsAsync();

        // 3. Transformation: Clean strings and cast to double for mathematical comparison
        var numericPrices = priceStrings
            .Select(p => double.Parse(p.Replace("$", ""), CultureInfo.InvariantCulture))
            .ToList();

        // Assert

        // 4. Assert: Verify the list is correctly ordered from most expensive to cheapest
        Assert.That(numericPrices, Is.Ordered.Descending);
    }

    // --- CART TESTS ---

    [Test]
    public async Task CART007_AddProductToCart_DisplaysCorrectDetailsOnCartPage()
    {
        // 1. Setup: Add a specific item to the cart and navigate to the Cart page
        await Page.Locator("[data-test='add-to-cart-sauce-labs-backpack']").ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();

        // Assert

        // 2. Assert: Verify the Name, Price, and Quantity '1' are correctly persisted in the cart view
        await Expect(Page.Locator("[data-test='inventory-item-name']")).ToHaveTextAsync("Sauce Labs Backpack");
        await Expect(Page.Locator("[data-test='inventory-item-price']")).ToHaveTextAsync("$29.99");
        await Expect(Page.Locator("[data-test='item-quantity']")).ToHaveTextAsync("1");
    }

    [Test]
    public async Task CART009_ClickCheckout_NavigatesToInformationPage()
    {
        // 1. Action: Navigate to the cart and click the Checkout button
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Page.Locator("[data-test='checkout']").ClickAsync();

        // Assert

        // 2. Assert: Verify the application redirects the user to 'Checkout: Your Information' (Step One)
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-step-one.html"));
    }

    // --- CHECKOUT TESTS ---

    [Test]
    public async Task CHKT001_SubmitValidInformation_NavigatesToOverviewPage()
    {
        // 1. Setup: Navigate to checkout and fill in the required user details
        await Page.Locator("[data-test='add-to-cart-sauce-labs-backpack']").ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Page.Locator("[data-test='checkout']").ClickAsync();

        await Page.Locator("[data-test='firstName']").FillAsync("John");
        await Page.Locator("[data-test='lastName']").FillAsync("Doe");
        await Page.Locator("[data-test='postalCode']").FillAsync("12345");

        // 2. Action: Click Continue to proceed to the review step
        await Page.Locator("[data-test='continue']").ClickAsync();

        // Assert

        // 3. Assert: Verify successful transition to the 'Checkout: Overview' page (Step Two)
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-step-two.html"));
    }

    [Test]
    public async Task CHKT006_CheckoutOverview_DisplaysCorrectSummaryInfo()
    {
        // 1. Setup: Use helper method to reach the final overview page
        await NavigateToCheckoutStepTwo();

        // Assert

        // 2. Assert: Verify payment information, shipping provider, and item identity are visible and correct
        await Expect(Page.Locator("[data-test='payment-info-value']")).ToBeVisibleAsync();
        await Expect(Page.Locator("[data-test='shipping-info-value']")).ToBeVisibleAsync();
        await Expect(Page.Locator("[data-test='inventory-item-name']")).ToHaveTextAsync("Sauce Labs Backpack");
    }

    [Test]
    public async Task CHKT007_CheckoutOverview_DisplaysCorrectSubtotal()
    {
        // 1. Setup: Reach the overview page
        await NavigateToCheckoutStepTwo();

        // Assert

        // 2. Assert: Verify the Item Total (Subtotal) matches the cost of the backpack before tax
        var subtotal = Page.Locator("[data-test='subtotal-label']");
        await Expect(subtotal).ToContainTextAsync("Item total: $29.99");
    }

    [Test]
    public async Task CHKT008_CheckoutOverview_DisplaysCorrectTaxAmount()
    {
        // 1. Setup: Reach the overview page
        await NavigateToCheckoutStepTwo();

        // Assert

        // 2. Assert: Verify the tax calculation displayed matches the expected value for the backpack
        var tax = Page.Locator("[data-test='tax-label']");
        await Expect(tax).ToContainTextAsync("Tax: $2.40");
    }

    [Test]
    public async Task CHKT009_CheckoutOverview_DisplaysCorrectTotalSum()
    {
        // 1. Setup: Reach the overview page
        await NavigateToCheckoutStepTwo();

        // Assert

        // 2. Assert: Verify the Grand Total (Subtotal + Tax) is mathematically correct and displayed
        var total = Page.Locator("[data-test='total-label']");
        await Expect(total).ToContainTextAsync("Total: $32.39");
    }

    /// <summary>
    /// Professional Helper Method: Orchestrates the prerequisite steps to reach the Checkout Overview page.
    /// Reduces code duplication across CHKT tests.
    /// </summary>
    private async Task NavigateToCheckoutStepTwo()
    {
        await Page.Locator("[data-test='add-to-cart-sauce-labs-backpack']").ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Page.Locator("[data-test='checkout']").ClickAsync();
        await Page.Locator("[data-test='firstName']").FillAsync("John");
        await Page.Locator("[data-test='lastName']").FillAsync("Doe");
        await Page.Locator("[data-test='postalCode']").FillAsync("12345");
        await Page.Locator("[data-test='continue']").ClickAsync();
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