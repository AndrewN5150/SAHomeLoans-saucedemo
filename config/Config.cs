using System;
using System.IO;
using SAHomeLoansSauceDemo.utils;

namespace SAHomeLoansSauceDemo.config;

public static class Config
{
    static Config()
    {
        var root = Directory.GetCurrentDirectory();
        var envPath = Path.Combine(root, ".env");
        
        // Try to find .env by traversing up
        var current = new DirectoryInfo(root);
        while (current != null)
        {
            var path = Path.Combine(current.FullName, ".env");
            if (File.Exists(path))
            {
                EnvLoader.Load(path);
                break;
            }
            current = current.Parent;
        }
    }

    public static string BaseUrl => Environment.GetEnvironmentVariable("BASE_URL") ?? "https://www.saucedemo.com/";
    public static string ApiBaseUrl => Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://reqres.in/api/";
    public static string Username => Environment.GetEnvironmentVariable("USERNAME") ?? "standard_user";
    public static string Password => Environment.GetEnvironmentVariable("PASSWORD") ?? "secret_sauce";
}
