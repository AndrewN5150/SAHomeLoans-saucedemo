using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SAHomeLoansSauceDemo.Config;

namespace SAHomeLoansSauceDemo.Tests;

public class BaseTest : PageTest
{
    protected ConfigSettings _config;

    [SetUp]
    public async Task BaseSetup()
    {
        // Load Configuration
        var configRoot = new ConfigurationBuilder()
        .SetBasePath(TestContext.CurrentContext.TestDirectory)
        .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
        .Build();


        _config = new ConfigSettings();
        configRoot.Bind(_config); // or individual binding if preferred

        // Navigate to BaseUrl.
        await Page.GotoAsync(_config.BaseUrl);
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        // 1. Identify File Info
        string screenshotName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        
        // 2. Dynamic Pathing
        string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo projectFolder = Directory.GetParent(assemblyPath).Parent.Parent.Parent;

        // 3. Routing
        string statusFolder = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed ? "Pass" : "Fail";
        string evidencePath = Path.Combine(projectFolder.FullName, "TestEvidence", statusFolder);

        // 4. Persistence
        if (!Directory.Exists(evidencePath)) Directory.CreateDirectory(evidencePath);
        string fullPath = Path.Combine(evidencePath, screenshotName);

        // Take screenshot
        await Page.ScreenshotAsync(new() { Path = fullPath, FullPage = true });
        TestContext.AddTestAttachment(fullPath, $"{statusFolder} Evidence");

        // Using explisit close
        await Context.CloseAsync();
    }
}
