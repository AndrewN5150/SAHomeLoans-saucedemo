namespace SAHomeLoansSauceDemo.Config;

public class ConfigSettings
{
    public string BaseUrl { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool Headless { get; set; } = true;
    public int SlowMo { get; set; } = 0;
}
