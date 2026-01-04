using FluentAssertions;
using NUnit.Framework;
using SAHomeLoansSauceDemo.Config;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SAHomeLoansSauceDemo.Tests;

public class ConfigTests
{
    [Test]
    public void DefaultConfig_ShouldLoadDevelopmentSettings()
    {
        // Act
        var configRoot = ConfigLoader.Load();
        var config = new ConfigSettings();
        configRoot.Bind(config);

        // Assert
        // Development has SlowMo 500
        config.SlowMo.Should().Be(500);
    }

    [Test]
    public void StagingConfig_ShouldLoadStagingSettings()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Staging");

        try
        {
            // Act
            var configRoot = ConfigLoader.Load();
            var config = new ConfigSettings();
            configRoot.Bind(config);

            // Assert
            // Staging has SlowMo 100
            config.SlowMo.Should().Be(100);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);
        }
    }

    [Test]
    public void ProductionConfig_ShouldLoadProductionSettings()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");

        try
        {
            // Act
            var configRoot = ConfigLoader.Load();
            var config = new ConfigSettings();
            configRoot.Bind(config);

            // Assert
            // Production has SlowMo 0 and Headless true
            config.SlowMo.Should().Be(0);
            config.Headless.Should().BeTrue();
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);
        }
    }
    
    [Test]
    public void EnvFile_ShouldOverride_IfLoaded()
    {
         // This test is tricky because DotNetEnv loads once globally usually, but we can try to test it.
         // Or we can test environment variable overrides since .env essentially sets env vars.
         
         // Arrange
         Environment.SetEnvironmentVariable("SlowMo", "999");
         
         try
         {
             var configRoot = ConfigLoader.Load();
             var config = new ConfigSettings();
             configRoot.Bind(config);
             
             // Env vars are added last in ConfigLoader, so they should win.
             config.SlowMo.Should().Be(999);
         }
         finally
         {
             Environment.SetEnvironmentVariable("SlowMo", null);
         }
    }
}
