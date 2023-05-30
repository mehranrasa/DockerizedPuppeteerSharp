using PuppeteerSharp;
using System.Runtime.InteropServices;

namespace DockerizedPuppeteerSharp.Services
{
    public class BrowserFactoryService : IBrowserFactoryService
    {
        private readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
        private IBrowser? browser = null;

        public async Task<IBrowser> GetBrowserAsync(string? proxyUrl = null, string? timeZone = null)
        {
            if (browser is not null && browser.IsConnected && !browser.IsClosed)
                return browser;

            var launchOptions = new LaunchOptions()
            {
                Headless = true,
                IgnoreHTTPSErrors = true,
                LogProcess = true,
                UserDataDir = "chrome_dir",
                Args = new[] 
                { 
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-infobars",
                    "--disable-dev-shm-usage" 
                }
            };
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                _ = launchOptions.Args.Append(string.Concat("--proxy-server=", proxyUrl));
                //sample proxy url: socks5://localhost:1080
            }

            if (!string.IsNullOrEmpty(timeZone))
            {
                launchOptions.Env.Add("TZ", timeZone);
                //sample time zone: Asia/Tehran
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //download chrome binaries on windows
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            }
            else 
            {
                //assuming chrome is already installed on host machine
                launchOptions.ExecutablePath = "/usr/bin/google-chrome-stable";
            }

            return browser = await Puppeteer.LaunchAsync(launchOptions);
        }

        public async Task<IBrowser> GetBrowserAsync(LaunchOptions launchOptions)
        {
            browser = await GetBrowserAsync();
            return browser;
        }

        public async Task<IPage> GetNewPageAsync()
        {
            browser = await GetBrowserAsync();

            var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync(userAgent);
            await page.SetViewportAsync(new ViewPortOptions()
            {
                Width = 1360,
                Height = 720
            });
            return page;
        }
    }
}
