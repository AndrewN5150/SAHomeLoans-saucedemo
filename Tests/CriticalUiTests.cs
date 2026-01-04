using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CriticalUiTests : PageTest
{
    [Test]
    public async Task AUTH001_AUTH007_LoginAndLogoutFlow()
    {
        // AUTH-001: Login Functionality
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");

        // AUTH-007: Logout Functionality
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Page.Locator("[data-test=\"logout-sidebar-link\"]").ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/");
    }

    [Test]
    public async Task CART001_AddToCartFromInventory()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // REQ-CART-001: Add to cart
        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Expect(Page.Locator("[data-test=\"remove-sauce-labs-backpack\"]")).ToHaveTextAsync("Remove");
        await Expect(Page.Locator("[data-test=\"shopping-cart-badge\"]")).ToHaveTextAsync("1");
    }

    [Test]
    public async Task CHKT011_CHKT012_CompletePurchase()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();

        // Add item and navigate to checkout
        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();
        await Page.Locator("[data-test=\"checkout\"]").ClickAsync();

        // Fill Info (REQ-CHKT-001)
        await Page.Locator("[data-test=\"firstName\"]").FillAsync("Test");
        await Page.Locator("[data-test=\"lastName\"]").FillAsync("User");
        await Page.Locator("[data-test=\"postalCode\"]").FillAsync("12345");
        await Page.Locator("[data-test=\"continue\"]").ClickAsync();

        // REQ-CHKT-011: Finish
        await Page.Locator("[data-test=\"finish\"]").ClickAsync();

        // REQ-CHKT-012: Confirmation message
        await Expect(Page.Locator("[data-test=\"complete-header\"]")).ToHaveTextAsync("Thank you for your order!");
    }
}