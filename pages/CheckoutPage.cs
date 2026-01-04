using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public class CheckoutPage : BasePage
{
    // Step 1: Information
    private ILocator FirstNameInput => _page.Locator("[data-test='firstName']");
    private ILocator LastNameInput => _page.Locator("[data-test='lastName']");
    private ILocator PostalCodeInput => _page.Locator("[data-test='postalCode']");
    private ILocator ContinueButton => _page.Locator("[data-test='continue']");

    // Step 2: Overview
    private ILocator PaymentInfo => _page.Locator("[data-test='payment-info-value']");
    private ILocator ShippingInfo => _page.Locator("[data-test='shipping-info-value']");
    private ILocator SubtotalLabel => _page.Locator("[data-test='subtotal-label']");
    private ILocator TaxLabel => _page.Locator("[data-test='tax-label']");
    private ILocator TotalLabel => _page.Locator("[data-test='total-label']");
    private ILocator FinishButton => _page.Locator("[data-test='finish']");
    
    // Step 3: Complete
    private ILocator CompleteHeader => _page.Locator("[data-test='complete-header']");

    public CheckoutPage(IPage page) : base(page) { }

    public async Task FillInformationAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync()
    {
        await ContinueButton.ClickAsync();
    }

    public async Task FinishAsync()
    {
        await FinishButton.ClickAsync();
    }

    // Getters for specific elements on Overview page for assertion
    public ILocator GetPaymentInfo() => PaymentInfo;
    public ILocator GetShippingInfo() => ShippingInfo;
    public ILocator GetSubtotalLabel() => SubtotalLabel;
    public ILocator GetTaxLabel() => TaxLabel;
    public ILocator GetTotalLabel() => TotalLabel;
}
