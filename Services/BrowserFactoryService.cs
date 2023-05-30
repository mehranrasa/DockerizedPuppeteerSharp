using PuppeteerSharp;

namespace DockerizedPuppeteerSharp.Services
{
    public class BrowserFactoryService : IBrowserFactoryService
    {
        private readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
        private IBrowser? browser = null;

        public async Task<IBrowser> GetBrowserAsync(string? proxyUrl = null, string? timeZone = null)
        {
            var launchOptions = new LaunchOptions()
            {
                ExecutablePath = "/usr/bin/google-chrome-stable",
                Headless = true,
                IgnoreHTTPSErrors = true,
                LogProcess = true,
                UserDataDir = "chrome_dir",
                Args = new[] { "--no-sandbox" }
            };

            if (!string.IsNullOrEmpty(proxyUrl))
            {
                //sample proxy url: socks5://localhost:1080
                _ = launchOptions.Args.Append(string.Concat("--proxy-server=", proxyUrl));
            }

            if (!string.IsNullOrEmpty(timeZone))
            {
                //sample time zone: Asia/Tehran
                launchOptions.Env.Add("TZ", timeZone);
            }

            browser ??= await Puppeteer.LaunchAsync(launchOptions);
            return browser;
        }

        public async Task<IBrowser> GetBrowserAsync(LaunchOptions launchOptions)
        {
            browser ??= await Puppeteer.LaunchAsync(launchOptions);
            return browser;
        }

        public async Task<IPage> GetNewPageAsync()
        {
            browser ??= await GetBrowserAsync();

            var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync(userAgent);
            await page.SetViewportAsync(new ViewPortOptions()
            {
                Width = 1360,
                Height = 720
            });

            return page;
        }

        public async ValueTask DisposeAsync()
        {
            if (browser is not null)
                await browser.CloseAsync();

            GC.SuppressFinalize(this);
        }
    }
}
