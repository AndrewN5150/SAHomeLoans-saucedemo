using Microsoft.Playwright;
using System.Threading.Tasks;

namespace SAHomeLoansSauceDemo.pages;

public class InventoryPage : BasePage
{
    public InventoryPage(IPage page) : base(page) { }

    public ILocator Title => Page.Locator("[data-test=\"title\"]");
    public ILocator InventoryItems => Page.Locator("[data-test=\"inventory-item\"]");
    public ILocator InventoryItemNames => Page.Locator("[data-test=\"inventory-item-name\"]");
    public ILocator InventoryItemDescriptions => Page.Locator("[data-test=\"inventory-item-desc\"]");
    public ILocator InventoryItemPrices => Page.Locator("[data-test=\"inventory-item-price\"]");
    public ILocator ShoppingCartLink => Page.Locator("[data-test=\"shopping-cart-link\"]");

    public async Task AddToCartAsync(string itemSlug)
    {
        await Page.Locator($"[data-test=\"add-to-cart-{itemSlug}\"]").ClickAsync();
    }

    public async Task GoToCartAsync()
    {
        await ShoppingCartLink.ClickAsync();
    }
}
