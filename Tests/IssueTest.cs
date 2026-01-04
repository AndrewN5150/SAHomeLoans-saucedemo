using NUnit.Framework;
using SAHomeLoansSauceDemo.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Linq;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class IssueTest : BaseTest
{
    private LoginPage _loginPage;
    private InventoryPage _inventoryPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;

    [SetUp]
    public async Task Setup()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutPage = new CheckoutPage(Page);
        
        // Ensure we are at base url (handled by BaseTest)
    }

    [Test]
    public async Task TC_ISSUE_001_Reproduce_BrokenImages()
    {
        // Login as problem_user to trigger UI issues
        await _loginPage.LoginAsync("problem_user", _config.Password);
        await _inventoryPage.WaitForPageToLoadAsync();

        // Requirement: Ensure images load correctly
        var image = Page.Locator(".inventory_item_img img").First;
        var imageSrc = await image.GetAttributeAsync("src");

        // Assert
        // This will FAIL because the actual src contains "sl-404.168ba304.jpg"
        Assert.That(imageSrc, Does.Not.Contain("sl-404"),
            "BUG REPRO: Product image failed to load correctly for problem_user.");
    }

    [Test]
    public async Task TC_ISSUE_002_Reproduce_SortingFailure()
    {
        await _loginPage.LoginAsync("problem_user", _config.Password);
        await _inventoryPage.WaitForPageToLoadAsync();

        // Requirement: Sort Name (Z to A)
        await _inventoryPage.SortProductsAsync("za");

        // We can expose a getter in InventoryPage for "First Product Name" if we want to be pure POM,
        // or just rely on generic getters.
        var names = await _inventoryPage.GetAllProductNamesAsync();
        var firstName = names.FirstOrDefault();

        // Assert
        // This will FAIL because sorting doesn't function correctly for this user
        Assert.That(firstName, Is.EqualTo("Test.allTheThings() T-Shirt (Red)"),
            "BUG REPRO: Sorting Name (Z to A) did not update the UI.");
    }

    [Test]
    public async Task TC_ISSUE_003_Reproduce_CartRemoveFailure()
    {
        await _loginPage.LoginAsync("problem_user", _config.Password);
        await _inventoryPage.WaitForPageToLoadAsync();

        // Add item then try to remove it
        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await Page.Locator("[data-test=\"remove-sauce-labs-backpack\"]").ClickAsync(); // Should add this to InventoryPage? YES.

        // Assert
        // This will FAIL because the remove action is broken for problem_user
        var cartBadge = Page.Locator("[data-test=\"shopping-cart-badge\"]");
        await Expect(cartBadge).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task TC_ISSUE_004_Reproduce_LastNameInputBug()
    {
        await _loginPage.LoginAsync("problem_user", _config.Password);

        await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");
        await Page.GotoAsync("https://www.saucedemo.com/checkout-step-one.html"); 
        // Or use POM methods to navigate?
        // _inventoryPage.GoToCartAsync() -> _cartPage.CheckoutAsync()
        // But using GotoAsync for shortcut is fine/faster for reproduction tests.
        // Let's stick to POM for consistency if possible, but Goto is ok.
        // Let's use POM flow:
        // await _inventoryPage.GoToCartAsync();
        // await _cartPage.CheckoutAsync(); // This might fail if problem_user has other issues? Assuming nav works.

        // Input data
        await _checkoutPage.FillInformationAsync("John", "Doe", ""); // We only assert LastName, but clean code fills others? 
        // The test filled: first "John", last "Doe". Postal? Original test didn't fill postal.
        // _checkoutPage.FillInformationAsync expects 3 args.
        // I should use the args.
        // Original: Fill first, fill last. Assert last.
        
        // I will use raw Playwright for specific "buggy" field interactions if the POM doesn't expose partial fills
        // OR I can just use FillInformationAsync with dummy postal.
        
        // _checkoutPage.FillInformationAsync("John", "Doe", "12345");
        
        // But wait, the test asserts "LastName field failed to accept or retain".
        // If I use the POM, it fills them all.
        // Let's try to be close to original.
        await _checkoutPage.FillInformationAsync("John", "Doe", "12345");

        // Assert
        var lastNameValue = await Page.Locator("[data-test=\"lastName\"]").InputValueAsync();
        Assert.That(lastNameValue, Is.EqualTo("Doe"),
            "BUG REPRO: Last Name field failed to accept or retain the correct string.");
    }

    [Test]
    public async Task TC_ISSUE_005_Reproduce_EmptyCartCheckout()
    {
        // Steps 1-6: Login
        await _loginPage.LoginAsync(_config.Username, _config.Password);

        // Steps 7-10: Navigate to Cart page with no items
        await _inventoryPage.GoToCartAsync();

        // Step 11: Click Checkout
        await _cartPage.CheckoutAsync();

        // Steps 12-15: Fill Info and Continue
        await _checkoutPage.FillInformationAsync("Test", "User", "12345");
        await _checkoutPage.ContinueAsync();

        // Step 16: Finish the order
        await _checkoutPage.FinishAsync();

        // Assert
        // BUG REPRO ASSERTION:
        var completeHeader = Page.Locator("[data-test=\"complete-header\"]");

        var isHeaderVisible = await completeHeader.IsVisibleAsync();
        Assert.That(isHeaderVisible, Is.False,
            "BUG REPRO: System allowed a checkout to complete with an empty cart. " +
            "Users should be blocked from initiating checkout if no products are present.");
    }

    [Test]
    public async Task TC_ISSUE_006_Reproduce_LoginPerformanceGlitch()
    {
        // using performance_glitch_user
        // await _loginPage.LoginAsync("performance_glitch_user", _config.Password);
        // But we need to measure time WITHIN the login action.
        // _loginPage.LoginAsync does the whole thing.
        // I can wrap it in timer.
        
        var startTime = DateTime.Now;
        await _loginPage.LoginAsync("performance_glitch_user", _config.Password);
        
        // LoginAsync waits for click to resolve.
        // But we also need to wait for nav? LoginAsync clicks, but doesn't explicitly wait for URL.
        // BasePage or LoginPage might not wait.
        // The original test did: Click, Expect URL.
        // LoginAsync does Click. Playwright auto-waits for click.
        // Expect URL waits for URL.
        
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        var duration = DateTime.Now - startTime;

        // Assert
        // This will FAIL because the glitch user takes ~5-10 seconds
        Assert.That(duration.TotalSeconds, Is.LessThan(3),
            $"BUG REPRO: Login took {duration.TotalSeconds} seconds, exceeding the 3s requirement.");
    }
}