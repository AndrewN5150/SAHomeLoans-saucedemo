using FluentAssertions.Execution;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Internal;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UITests : PageTest
{
  //I have moved the commonly used Login flow to its own Method, to reduce duplication of code.
  public async void Login()
  {
    await Page.GotoAsync("https://www.saucedemo.com/");

    //Enter username
    await Page.GetByPlaceholder("Username").FillAsync("standard_user");
    //Enter password
    await Page.GetByPlaceholder("Password").FillAsync("secret_sauce");
    //Click Submit
    await Page.GetByText("Login").ClickAsync();
  }

  [Test]
  public async Task GivenValidCredentials_WhenUserLogsIn_ThenLoginIsSuccessful()
  {
    //Arrange
    Login();

    //Assert
    using (new AssertionScope())
    {
      await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
      await Expect(Page).ToHaveTitleAsync("Swag Labs");

      await Expect(Page.Locator("[data-test=\"title\"]")).ToHaveTextAsync("Products");
      await Expect(Page.Locator("[data-test=\"inventory-item-description\"]")).ToHaveCountAsync(6);
      await Expect(Page.Locator("[data-test=\"inventory-item-name\"]")).ToContainTextAsync(new string[] { "Sauce Labs Backpack", "Sauce Labs Bike Light", "Sauce Labs Bolt T-Shirt", "Sauce Labs Fleece Jacket", "Sauce Labs Onesie", "Test.allTheThings() T-Shirt (Red)" });
      await Expect(Page.Locator("[data-test=\"inventory-item-desc\"]")).ToContainTextAsync(new string[] { "carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection.", "A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included.", "Get your testing superhero on with the Sauce Labs bolt T-shirt. From American Apparel, 100% ringspun combed cotton, heather gray with red bolt.", "It's not every day that you come across a midweight quarter-zip fleece jacket capable of handling everything from a relaxing day outdoors to a busy day at the office.", "Rib snap infant onesie for the junior automation engineer in development. Reinforced 3-snap bottom closure, two-needle hemmed sleeved and bottom won't unravel.", "This classic Sauce Labs t-shirt is perfect to wear when cozying up to your keyboard to automate a few tests. Super-soft and comfy ringspun combed cotton." });
      await Expect(Page.Locator("[data-test=\"inventory-item-price\"]")).ToContainTextAsync(new string[] { "$29.99", "$9.99", "$15.99", "$49.99", "7.99", "$15.99" });
    }
  }

  [Test]
  public async Task GivenItemsInCart_WhenNavigatingToCheckout_ThenOverviewIsCorrect()
  {
    //Arrange
    Login();

    //Act

    //Add 2 items to the cart
    await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
    await Page.Locator("[data-test=\"add-to-cart-sauce-labs-bike-light\"]").ClickAsync();

    //Click on the shopping cart link "strangly this is a link and not a button"
    await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();

    //Click Checkout
    await Page.GetByText("Checkout").ClickAsync();

    //Enter First Name
    await Page.GetByPlaceholder("First Name").FillAsync("Andrew");
    //Enter Last Name
    await Page.GetByPlaceholder("Last Name").FillAsync("Neilon");
    //Enter Zip/Postal Code
    await Page.GetByPlaceholder("Zip/Postal Code").FillAsync("4051");

    //Click Continue
    await Page.GetByText("Continue").ClickAsync();

    //Assert
    using (new AssertionScope())
    {
      await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/checkout-step-two.html");
      await Expect(Page).ToHaveTitleAsync("Swag Labs");

      await Expect(Page.Locator("[data-test=\"title\"]")).ToHaveTextAsync("Checkout: Overview");
      await Expect(Page.Locator("[data-test=\"inventory-item\"]")).ToHaveCountAsync(2);

      await Expect(Page.Locator("[data-test=\"inventory-item-name\"]")).ToContainTextAsync(new string[] { "Sauce Labs Backpack", "Sauce Labs Bike Light" });
      await Expect(Page.Locator("[data-test=\"inventory-item-desc\"]")).ToContainTextAsync(new string[] { "carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection.", "A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included." });
      await Expect(Page.Locator("[data-test=\"inventory-item-price\"]")).ToContainTextAsync(new string[] { "$29.99", "$9.99" });

      await Expect(Page.Locator("[data-test=\"payment-info-label\"]")).ToHaveTextAsync("Payment Information:");
      await Expect(Page.Locator("[data-test=\"payment-info-value\"]")).ToHaveTextAsync("SauceCard #31337");

      await Expect(Page.Locator("[data-test=\"shipping-info-label\"]")).ToHaveTextAsync("Shipping Information:");
      await Expect(Page.Locator("[data-test=\"shipping-info-value\"]")).ToHaveTextAsync("Free Pony Express Delivery!");

      await Expect(Page.Locator("[data-test=\"total-info-label\"]")).ToHaveTextAsync("Price Total");
      await Expect(Page.Locator("[data-test=\"subtotal-label\"]")).ToHaveTextAsync("Item total: $39.98");
      await Expect(Page.Locator("[data-test=\"tax-label\"]")).ToHaveTextAsync("Tax: $3.20");
      await Expect(Page.Locator("[data-test=\"total-label\"]")).ToHaveTextAsync("Total: $43.18");
    }
  }

  [Test]
  public async Task PurchaseAndPaymeGivenItemsInCart_WhenUserCompletesPurchase_ThenPaymentIsSuccessfulntFlowTest()
  {
    //Arrange
    Login();

    //Act

    //Add 2 items to the cart
    await Page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
    await Page.Locator("[data-test=\"add-to-cart-sauce-labs-bike-light\"]").ClickAsync();

    //Click on the shopping cart link "strangly this is a link and not a button"
    await Page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();

    //Click Checkout
    await Page.GetByText("Checkout").ClickAsync();

    //Enter First Name
    await Page.GetByPlaceholder("First Name").FillAsync("Andrew");
    //Enter Last Name
    await Page.GetByPlaceholder("Last Name").FillAsync("Neilon");
    //Enter Zip/Postal Code
    await Page.GetByPlaceholder("Zip/Postal Code").FillAsync("4051");

    //Click Continue
    await Page.GetByText("Continue").ClickAsync();

    //Click Finish
    await Page.GetByText("Finish").ClickAsync();

    //Assert
    using (new AssertionScope())
    {
      await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/checkout-complete.html");
      await Expect(Page).ToHaveTitleAsync("Swag Labs");

      await Expect(Page.Locator("[data-test=\"title\"]")).ToHaveTextAsync("Checkout: Complete!");
      await Expect(Page.Locator("[data-test=\"complete-header\"]")).ToHaveTextAsync("Thank you for your order!");
      await Expect(Page.Locator("[data-test=\"complete-text\"]")).ToHaveTextAsync("Your order has been dispatched, and will arrive just as fast as the pony can get there!");
      await Expect(Page.Locator("[data-test=\"back-to-products\"]")).ToHaveTextAsync("Back Home");
    }
  }
}
