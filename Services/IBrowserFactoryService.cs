using PuppeteerSharp;

namespace DockerizedPuppeteerSharp.Services
{
    public interface IBrowserFactoryService
    {
        Task<IBrowser> GetBrowserAsync(LaunchOptions launchOptions);
        Task<IBrowser> GetBrowserAsync(string? proxyUrl = null, string? timeZone = null);
        Task<IPage> GetNewPageAsync();
    }
}
