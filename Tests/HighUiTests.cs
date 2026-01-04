using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HightUiTests : PageTest
{
    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync("https://www.saucedemo.com/");
        await Page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await Page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await Page.Locator("[data-test=\"login-button\"]").ClickAsync();
    }

    [Test]
    public async Task PROD001_VerifyProductDisplay()
    {
        // REQ-PROD-001: 6 items displayed
        var inventoryItems = Page.Locator("[data-test=\"inventory-item\"]");
        await Expect(inventoryItems).ToHaveCountAsync(6);
    }

    [Test]
    public async Task PROD003_006_SortingFunctionality()
    {
        // REQ-PROD-005: Price Low to High
        await Page.Locator("[data-test=\"product-sort-container\"]").SelectOptionAsync("lohi");
        var firstPrice = await Page.Locator("[data-test=\"inventory-item-price\"]").First.InnerTextAsync();
        Assert.That(firstPrice, Is.EqualTo("$7.99")); // Sauce Labs Onesie price

        // REQ-PROD-004: Name Z to A
        await Page.Locator("[data-test=\"product-sort-container\"]").SelectOptionAsync("za");
        var firstName = await Page.Locator("[data-test=\"inventory-item-name\"]").First.InnerTextAsync();
        Assert.That(firstName, Is.EqualTo("Test.allTheThings() T-Shirt (Red)"));
    }

    [Test]
    public async Task CHKT006_009_VerifyOrderTotals()
    {
        await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();
        await Page.Locator("[data-test=\"checkout\"]").ClickAsync();

        await Page.Locator("[data-test=\"firstName\"]").FillAsync("John");
        await Page.Locator("[data-test=\"lastName\"]").FillAsync("Doe");
        await Page.Locator("[data-test=\"postalCode\"]").FillAsync("90210");
        await Page.Locator("[data-test=\"continue\"]").ClickAsync();

        // REQ-CHKT-007, 008, 009: Totals
        await Expect(Page.Locator("[data-test=\"subtotal-label\"]")).ToContainTextAsync("Item total: $29.99");
        await Expect(Page.Locator("[data-test=\"tax-label\"]")).ToContainTextAsync("Tax: $2.40");
        await Expect(Page.Locator("[data-test=\"total-label\"]")).ToContainTextAsync("Total: $32.39");
    }
}