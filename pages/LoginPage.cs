using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public class LoginPage : BasePage
{
    public ILocator UsernameInput => _page.Locator("[data-test='username']");
    public ILocator PasswordInput => _page.Locator("[data-test='password']");
    public ILocator LoginButton => _page.Locator("[data-test='login-button']");

    public LoginPage(IPage page) : base(page) { }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public override async Task WaitForPageToLoadAsync()
    {
        await LoginButton.WaitForAsync();
    }
}
