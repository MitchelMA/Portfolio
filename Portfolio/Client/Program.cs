using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portfolio.Factories;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
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
        builder.Services.AddScoped<LangTablePreCacher>();
        builder.Services.AddScoped<HeroInfoGetter>();

        builder.Services.AddScoped<IMapper<ProjectDataModel, CarouselModel>, ToCarouselModelMapper>();
        builder.Services.AddScoped<IMapper<LangHeaderModel, HeaderData>, ToHeaderDataMapper>();
        builder.Services.AddScoped<IMapper<NewProjectMetaDataModel, HeaderData>, ToNewHeaderDataMapper>();
        builder.Services.AddScoped<IMapper<LangLinkModel, NavLinkData>, ToNavLinkDataMapper>();

        builder.Services.AddSingleton<AppState>();
        builder.Services.AddSingleton<LanguageTable>();
        builder.Services.AddSingleton<ContentProvider>();

        builder.Services.AddTransient<LightboxRegistry>();
        builder.Services.AddTransient<EnlargeImageService>();
        builder.Services.AddMemoryCache();

        await builder.Build().RunAsync();
    }
}