using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public class InventoryPage : BasePage
{
    // Locators
    private ILocator ProductSortDropdown => _page.Locator("[data-test='product-sort-container']");
    public ILocator InventoryItems => _page.Locator("[data-test='inventory-item']");
    private ILocator ShoppingCartLink => _page.Locator("[data-test='shopping-cart-link']");

    public InventoryPage(IPage page) : base(page) { }

    public async Task<int> GetProductCountAsync()
    {
        return await InventoryItems.CountAsync();
    }

    public ILocator GetProductByName(string name)
    {
        return InventoryItems.Filter(new() { HasText = name });
    }

    public async Task SortProductsAsync(string option)
    {
        await ProductSortDropdown.SelectOptionAsync(option);
    }

    public async Task<IReadOnlyList<string>> GetAllProductNamesAsync()
    {
        return await _page.Locator("[data-test='inventory-item-name']").AllInnerTextsAsync();
    }

    public async Task<IReadOnlyList<string>> GetAllProductPricesAsync()
    {
        return await _page.Locator("[data-test='inventory-item-price']").AllInnerTextsAsync();
    }

    public async Task AddProductToCartAsync(string productId)
    {
        // e.g. "sauce-labs-backpack" from "add-to-cart-sauce-labs-backpack"
        await _page.Locator($"[data-test='add-to-cart-{productId}']").ClickAsync();
    }

    public async Task GoToCartAsync()
    {
        await ShoppingCartLink.ClickAsync();
    }

    public override async Task WaitForPageToLoadAsync()
    {
        // Wait for at least one item or the container to verify load
        await _page.Locator(".inventory_list").WaitForAsync();
    }
}
