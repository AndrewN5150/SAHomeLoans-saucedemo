using Microsoft.Playwright;
using SAHomeLoansSauceDemo.config;
using System.Threading.Tasks;

namespace SAHomeLoansSauceDemo.pages;

public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) { }

    public async Task NavigateAsync()
    {
        await Page.GotoAsync(Config.BaseUrl);
    }

    public async Task LoginAsync(string username, string password)
    {
        await Page.GetByPlaceholder("Username").FillAsync(username);
        await Page.GetByPlaceholder("Password").FillAsync(password);
        await Page.GetByText("Login").ClickAsync();
    }
}
