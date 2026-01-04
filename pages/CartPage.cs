using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public class CartPage : BasePage
{
    // Cart page items use the site's cart item class
    private ILocator CartItems => _page.Locator(".cart_item");
    private ILocator CheckoutButton => _page.Locator("[data-test='checkout']");
    private ILocator ContinueShoppingButton => _page.Locator("[data-test='continue-shopping']");

    public CartPage(IPage page) : base(page) { }

    public ILocator GetCartItemByName(string name)
    {
        return CartItems.Filter(new() { HasText = name });
    }

    public async Task CheckoutAsync()
    {
        await CheckoutButton.ClickAsync();
    }

    public async Task ContinueShoppingAsync()
    {
        await ContinueShoppingButton.ClickAsync();
    }

    public override async Task WaitForPageToLoadAsync()
    {
        await CheckoutButton.WaitForAsync();
    }
}
