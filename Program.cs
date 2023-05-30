using DockerizedPuppeteerSharp.Services;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IBrowserFactoryService, BrowserFactoryService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var browserService = scope.ServiceProvider.GetRequiredService<IBrowserFactoryService>();

    var browser = await browserService.GetBrowserAsync();
    using var page = await browserService.GetNewPageAsync();

    var response = await page.GoToAsync("https://google.com",
        WaitUntilNavigation.DOMContentLoaded | WaitUntilNavigation.Networkidle2);

    response.Headers.ToList().ForEach(h => Console.WriteLine(h.Key + '=' + h.Value));
}

// Configure the HTTP request pipeline.

app.Run();