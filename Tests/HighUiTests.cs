using NUnit.Framework;
using SAHomeLoansSauceDemo.Pages;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HighUiTests : BaseTest
{
    private LoginPage _loginPage;
    private InventoryPage _inventoryPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;

    [SetUp]
    public async Task Setup()
    {
        // 1. Navigate to the login page before each test
        _loginPage = new LoginPage(Page);
        // 2. Initialize Inventory page objects
        _inventoryPage = new InventoryPage(Page);
        // 3. Initialize Cart and Checkout pages
        _cartPage = new CartPage(Page);
        _checkoutPage = new CheckoutPage(Page);


        // Perform Login as part of setup for these tests
        await _loginPage.LoginAsync(_config.Username, _config.Password);
    }

    // --- INVENTORY TESTS ---

    [Test]
    public async Task PROD001_VerifyAllProducts_DisplaysCorrectCountAndInfo()
    {
        // 1. Using an Array to store the individual item details
        var expectedProducts = new[]
        {
            new { Name = "Sauce Labs Backpack", Price = "$29.99", Desc = "carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection." },
            new { Name = "Sauce Labs Bike Light", Price = "$9.99", Desc = "A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included." },
            new { Name = "Sauce Labs Bolt T-Shirt", Price = "$15.99", Desc = "Get your testing superhero on with the Sauce Labs bolt T-shirt. From American Apparel, 100% ringspun combed cotton, heather gray with red bolt." },
            new { Name = "Sauce Labs Fleece Jacket", Price = "$49.99", Desc = "It's not every day that you come across a midweight quarter-zip fleece jacket capable of handling everything from a relaxing day outdoors to a busy day at the office." },
            new { Name = "Sauce Labs Onesie", Price = "$7.99", Desc = "Rib snap infant onesie for the junior automation engineer in development. Reinforced 3-snap bottom closure, two-needle hemmed sleeved and bottom won't unravel." },
            new { Name = "Test.allTheThings() T-Shirt (Red)", Price = "$15.99", Desc = "This classic Sauce Labs t-shirt is perfect to wear when cozying up to your keyboard to automate a few tests. Super-soft and comfy ringspun combed cotton." }
        };

        // 2. Ensure the catalog contains exactly 6 items
        await Expect(_inventoryPage.InventoryItems).ToHaveCountAsync(6);

        // 3. Validate each item
        foreach (var expected in expectedProducts)
        {
            var item = _inventoryPage.GetProductByName(expected.Name);
            await Expect(item).ToBeVisibleAsync();
            
            await Expect(item.Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(expected.Name);
            await Expect(item.Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(expected.Price);
            await Expect(item.Locator("[data-test='inventory-item-desc']")).ToHaveTextAsync(expected.Desc);
            await Expect(item.Locator(".inventory_item_img img")).ToBeVisibleAsync();
        }
    }

    // --- INVENTORY SORTING TESTS ---

    [Test]
    public async Task PROD003_SortByNameAToZ_UpdatesListOrder()
    {
        // 1. Sort the products from A to Z
        await _inventoryPage.SortProductsAsync("az");
        // 2. Capture all product names from the UI
        var names = await _inventoryPage.GetAllProductNamesAsync();
        // 3. Compare UI order against a mathematically sorted list
        var expectedOrder = names.OrderBy(x => x).ToList();
        Assert.That(names, Is.EqualTo(expectedOrder), "Products not sorted A-Z");
    }

    [Test]
    public async Task PROD004_SortByNameZToA_UpdatesListOrder()
    {
        // 1. Sort the products from Z to A
        await _inventoryPage.SortProductsAsync("za");
        // 2. Capture all product names from the UI
        var names = await _inventoryPage.GetAllProductNamesAsync();
        // 3. Compare UI order against a descending sorted list
        var expectedOrder = names.OrderByDescending(x => x).ToList();
        Assert.That(names, Is.EqualTo(expectedOrder), "Products not sorted Z-A");
    }

    [Test]
    public async Task PROD005_SortByPriceLowToHigh_UpdatesListOrder()
    {
        // 1. Sort products by price low to high
        await _inventoryPage.SortProductsAsync("lohi");
        // 2. Capture and convert price strings to a numerical list
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var numericPrices = prices
            .Select(p => double.Parse(p.Replace("$", ""), CultureInfo.InvariantCulture))
            .ToList();
        // 3. Validate that the numbers are in ascending order
        Assert.That(numericPrices, Is.Ordered.Ascending);
    }

    [Test]
    public async Task PROD006_SortByPriceHighToLow_UpdatesListOrder()
    {
        // 1. Sort products by price high to low
        await _inventoryPage.SortProductsAsync("hilo");
        // 2. Capture and convert price strings to a numerical list
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var numericPrices = prices
            .Select(p => double.Parse(p.Replace("$", ""), CultureInfo.InvariantCulture))
            .ToList();
        // 3. Validate that the numbers are in descending order
        Assert.That(numericPrices, Is.Ordered.Descending);
    }

    // --- CART TESTS ---

    [Test]
    public async Task CART007_AddProductToCart_DisplaysCorrectDetailsOnCartPage()
    {
        // 1. Add item to cart and navigate to the cart page
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await _inventoryPage.GoToCartAsync();

        // 2. Identify the specific item in the cart
        var cartItem = _cartPage.GetCartItemByName("Sauce Labs Backpack");

        // 3. Verify the product details and quantity match
        await Expect(cartItem).ToHaveCountAsync(1);
        await Expect(cartItem.Locator(".inventory_item_price")).ToHaveTextAsync("$29.99");
        await Expect(cartItem.Locator(".cart_quantity")).ToHaveTextAsync("1");
    }

    [Test]
    public async Task CART009_ClickCheckout_NavigatesToInformationPage()
    {
        // 1. Go to the cart
        await _inventoryPage.GoToCartAsync();
        // 2. Click the checkout button
        await _cartPage.CheckoutAsync();
        // 3. Verify navigation to the info input page
        await _cartPage.WaitForURLAsync(".*checkout-step-one.html");
    }

    // --- CHECKOUT TESTS ---

    [Test]
    public async Task CHKT001_SubmitValidInformation_NavigatesToOverviewPage()
    {
        // 1. Reach the checkout information page
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.CheckoutAsync();

        // 2. Fill in the required user details and continue
        await _checkoutPage.FillInformationAsync("John", "Doe", "12345");
        await _checkoutPage.ContinueAsync();

        // 3. Verify navigation to the final overview step
        await _checkoutPage.WaitForURLAsync(".*checkout-step-two.html");
    }

    [Test]
    public async Task CHKT006_CheckoutOverview_DisplaysCorrectSummaryInfo()
    {
        // 1. Navigate to the checkout summary page
        await NavigateToCheckoutStepTwo();

        // 2. Verify visibility of payment and shipping info
        await Expect(_checkoutPage.GetPaymentInfo()).ToBeVisibleAsync();
        await Expect(_checkoutPage.GetShippingInfo()).ToBeVisibleAsync();
        // 3. Confirm the correct product is listed in the summary
        await Expect(Page.Locator("[data-test='inventory-item-name']")).ToHaveTextAsync("Sauce Labs Backpack");
    }

    [Test]
    public async Task CHKT007_CheckoutOverview_DisplaysCorrectSubtotal()
    {
        // 1. Navigate to the checkout summary page
        await NavigateToCheckoutStepTwo();
        // 2. Check the subtotal amount
        await Expect(_checkoutPage.GetSubtotalLabel()).ToContainTextAsync("Item total: $29.99");
    }

    [Test]
    public async Task CHKT008_CheckoutOverview_DisplaysCorrectTaxAmount()
    {
        // 1. Navigate to the checkout summary page
        await NavigateToCheckoutStepTwo();
        // 2. Check the tax amount
        await Expect(_checkoutPage.GetTaxLabel()).ToContainTextAsync("Tax: $2.40");
    }

    [Test]
    public async Task CHKT009_CheckoutOverview_DisplaysCorrectTotalSum()
    {
        // 1. Navigate to the checkout summary page
        await NavigateToCheckoutStepTwo();
        // 2. Check the final total sum
        await Expect(_checkoutPage.GetTotalLabel()).ToContainTextAsync("Total: $32.39");
    }

    private async Task NavigateToCheckoutStepTwo()
    {
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.CheckoutAsync();
        await _checkoutPage.FillInformationAsync("John", "Doe", "12345");
        await _checkoutPage.ContinueAsync();
    }
}
