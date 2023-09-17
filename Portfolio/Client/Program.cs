using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portfolio.Factories;
using Portfolio.Services;

namespace Portfolio.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<ColourMapperFactory>();
        builder.Services.AddScoped<ProjectInfoGetter>();

        builder.Services.AddSingleton<AppState>();
        builder.Services.AddSingleton<ProjectState>();
        builder.Services.AddSingleton<LanguageTable>();

        builder.Services.AddTransient<LightboxRegistry>();

        await builder.Build().RunAsync();
    }
}