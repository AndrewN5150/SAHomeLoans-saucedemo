using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SAHomeLoansSauceDemo.Config;

public static class ConfigLoader
{
    public static IConfiguration Load()
    {
        // 1. Load .env file (if exists)
        DotNetEnv.Env.Load();

        // 2. Determine environment
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                             ?? Environment.GetEnvironmentVariable("APP_ENVIRONMENT") 
                             ?? "Development";

        Console.WriteLine($"[ConfigLoader] Loading configuration for environment: {environment}");

        // 3. Build Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
