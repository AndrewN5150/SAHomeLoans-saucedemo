using NUnit.Framework;
using SAHomeLoansSauceDemo.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CriticalUiTests : BaseTest
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
    }

    // --- AUTHENTICATION TESTS ---

    [Test]
    public async Task AUTH001_ValidUser_RedirectsToInventory()
    {
        // 1. Action: Perform login with valid 'standard_user' credentials
        await _loginPage.LoginAsync(_config.Username, _config.Password);

        // 2. Assert: Verify the user is redirected to the products inventory page
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task AUTH007_Logout_RedirectsToLoginPage()
    {
        // 1. Setup: Navigate and log in
        await _loginPage.LoginAsync(_config.Username, _config.Password);

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
        // 1. Setup: Log in
        await _loginPage.LoginAsync(_config.Username, _config.Password);

        // 2. Action: Click the 'Add to cart' button for the backpack
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");

        // 3. Assert: Verify the button text changes to 'Remove' and the cart badge increments to '1'
        await Expect(Page.Locator("[data-test=\"remove-sauce-labs-backpack\"]")).ToHaveTextAsync("Remove");
        await Expect(Page.Locator("[data-test=\"shopping-cart-badge\"]")).ToHaveTextAsync("1");
    }

    // --- CHECKOUT TESTS ---

    [Test]
    public async Task CHKT011_CHKT012_CompleteCheckout_DisplaysThankYouMessage()
    {
        // 1. Setup: Log in and navigate through the cart to the checkout details page
        await _loginPage.LoginAsync(_config.Username, _config.Password);
        
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.CheckoutAsync();

        // 2. Action: Provide shipping information and finalize the purchase
        await _checkoutPage.FillInformationAsync("Test", "User", "12345");
        await _checkoutPage.ContinueAsync();

        // REQ-CHKT-011: Submit the final order
        await _checkoutPage.FinishAsync();

        // 3. Assert (REQ-CHKT-012): Verify the order completion header and descriptive text are displayed
        await Expect(Page.Locator("[data-test=\"complete-header\"]")).ToHaveTextAsync("Thank you for your order!");
        await Expect(Page.Locator("[data-test=\"complete-text\"]")).ToHaveTextAsync("Your order has been dispatched, and will arrive just as fast as the pony can get there!");
    }
}