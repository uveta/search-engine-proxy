namespace Proxy.Engine.Playwright;

using Microsoft.Playwright;

public static class PlaywrightExtensions
{
    public static IServiceCollection AddPlaywright(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
            var chromium = playwright.Chromium;
            // Can be "msedge", "chrome-beta", "msedge-beta", "msedge-dev", etc.
            var browser = chromium.LaunchAsync(new BrowserTypeLaunchOptions { Channel = "chrome" }).GetAwaiter().GetResult();
            return browser;
        });
        return services;
    }
}
