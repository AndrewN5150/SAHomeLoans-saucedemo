using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public class LoginPage : BasePage
{
    private ILocator UsernameInput => _page.Locator("[data-test='username']");
    private ILocator PasswordInput => _page.Locator("[data-test='password']");
    private ILocator LoginButton => _page.Locator("[data-test='login-button']");

    public LoginPage(IPage page) : base(page) { }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
