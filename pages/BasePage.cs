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

    /// <summary>
    /// Explicitly wait for a specific element that signifies the page is fully loaded.
    /// Derived classes should override this to wait for their unique element.
    /// </summary>
    public virtual async Task WaitForPageToLoadAsync()
    {
        await Task.CompletedTask; // Default implementation does nothing
    }
}
