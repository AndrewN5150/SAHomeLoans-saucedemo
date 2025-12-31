using Microsoft.Playwright;
using System.Threading.Tasks;

namespace SAHomeLoansSauceDemo.pages;

public class CartPage : BasePage
{
    public CartPage(IPage page) : base(page) { }

    public ILocator CheckoutButton => Page.GetByText("Checkout");

    public async Task CheckoutAsync()
    {
        await CheckoutButton.ClickAsync();
    }
}
