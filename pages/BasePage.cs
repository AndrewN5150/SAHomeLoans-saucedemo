using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.Pages;

public abstract class BasePage
{
    protected readonly IPage _page;

    protected BasePage(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Common navigation or validation methods can go here.
    /// </summary>
    public async Task WaitForURLAsync(string urlRegex)
    {
        await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex(urlRegex));
    }
}
