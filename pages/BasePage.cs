using Microsoft.Playwright;

namespace SAHomeLoansSauceDemo.pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }
}
