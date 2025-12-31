using Microsoft.Playwright;
using System.Threading.Tasks;

namespace SAHomeLoansSauceDemo.pages;

public class CheckoutPage : BasePage
{
    public CheckoutPage(IPage page) : base(page) { }

    public ILocator Title => Page.Locator("[data-test=\"title\"]");
    
    // Step 1
    public async Task EnterInformationAsync(string firstName, string lastName, string zipCode)
    {
        await Page.GetByPlaceholder("First Name").FillAsync(firstName);
        await Page.GetByPlaceholder("Last Name").FillAsync(lastName);
        await Page.GetByPlaceholder("Zip/Postal Code").FillAsync(zipCode);
    }

    public async Task ContinueAsync()
    {
        await Page.GetByText("Continue").ClickAsync();
    }

    // Step 2 (Overview)
    public ILocator PaymentInfoLabel => Page.Locator("[data-test=\"payment-info-label\"]");
    public ILocator PaymentInfoValue => Page.Locator("[data-test=\"payment-info-value\"]");
    public ILocator ShippingInfoLabel => Page.Locator("[data-test=\"shipping-info-label\"]");
    public ILocator ShippingInfoValue => Page.Locator("[data-test=\"shipping-info-value\"]");
    public ILocator TotalInfoLabel => Page.Locator("[data-test=\"total-info-label\"]");
    public ILocator SubtotalLabel => Page.Locator("[data-test=\"subtotal-label\"]");
    public ILocator TaxLabel => Page.Locator("[data-test=\"tax-label\"]");
    public ILocator TotalLabel => Page.Locator("[data-test=\"total-label\"]");
    public ILocator InventoryItems => Page.Locator("[data-test=\"inventory-item\"]");
    public ILocator InventoryItemNames => Page.Locator("[data-test=\"inventory-item-name\"]");
    public ILocator InventoryItemDescriptions => Page.Locator("[data-test=\"inventory-item-desc\"]");
    public ILocator InventoryItemPrices => Page.Locator("[data-test=\"inventory-item-price\"]");

    public async Task FinishAsync()
    {
        await Page.GetByText("Finish").ClickAsync();
    }

    // Complete
    public ILocator CompleteHeader => Page.Locator("[data-test=\"complete-header\"]");
    public ILocator CompleteText => Page.Locator("[data-test=\"complete-text\"]");
    public ILocator BackToProductsButton => Page.Locator("[data-test=\"back-to-products\"]");
}
