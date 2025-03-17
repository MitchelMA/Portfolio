using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Model.Project;
using Portfolio.Model.Text;

namespace Portfolio.Services;

public sealed class ContentProvider
{
    private readonly string _contentLocation;
    private readonly string _projectsSubPath;
    private readonly string _heroesSubPath;

    // TODO! Eventualy replace with the Language table,
    // when the language table has been repurposed for only language switching
    // and keeping track of changes
    private string[] _supportedLanguages = Array.Empty<string>();

    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navManager;
    private readonly AppState _appState;

    public delegate Task SupportedLanguagesLoadedAsync();
    public event SupportedLanguagesLoadedAsync? OnSupportedLanguagesLoadedAsync;

    public ContentProvider(IConfiguration configuration, IMemoryCache memoryCache, NavigationManager navManager, AppState appState)
    {
        _contentLocation = configuration["ContentLocation:Base"];
        _projectsSubPath = configuration["ContentLocation:Projects"];
        _heroesSubPath   = configuration["ContentLocation:Heroes"];

        _memoryCache = memoryCache;
        _httpClient = new HttpClient();
        _navManager = navManager;
        _appState = appState;
        Task.Run(async () => await LoadSupportedLanguages());
    }

    private async Task LoadSupportedLanguages()
    {
        if (_supportedLanguages.Length > 0)
            return;

        _supportedLanguages = (await _httpClient.GetFromJsonAsync<string[]>(
            Path.Combine(_navManager.BaseUri, "Text", "SupportedLanguages.json")
        ))!;
        if (OnSupportedLanguagesLoadedAsync is not null)
            await OnSupportedLanguagesLoadedAsync!.Invoke();
    }

    public async Task AwaitSupportedLanguages(SupportedLanguagesLoadedAsync onReady)
    {
        if (_supportedLanguages.Length > 0)
        {
            await onReady.Invoke();
            return;
        }

        OnSupportedLanguagesLoadedAsync += onReady;
    }

    private string CultureToName(int cultureIdx)
    {
        return _supportedLanguages[cultureIdx];
    }

    public async Task<NewProjectModel> GetProjectData(string informalName)
    {
        var cached = await _memoryCache.GetOrCreateAsync($"projects/{informalName}",
            async entry => {
                var message = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri(_contentLocation + _projectsSubPath + informalName + "?valueOnly=true")
                ).SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);
                var response = await _httpClient.SendAsync(message);
                return await response.Content.ReadFromJsonAsync<NewProjectModel>();
            });

        return cached;
    }

    public async Task<NewProjectMetaDataModel> GetProjectMeta(string informalName)
    {
        var cached = await _memoryCache.GetOrCreateAsync($"projects/meta/{informalName}/{_appState.CurrentLanguage}",
            async entry => {
                var message = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri(_contentLocation + _projectsSubPath + informalName + $"/meta-data/{CultureToName(_appState.CurrentLanguage)}?valueOnly=true")
                ).SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);
                var response = await _httpClient.SendAsync(message);
                return await response.Content.ReadFromJsonAsync<NewProjectMetaDataModel>();
            });

        return cached;
    }

    public async Task<string> GetProjectPageContent(string informalName)
    {
        Task<string?> cached = _memoryCache.GetOrCreateAsync($"projects/text/{informalName}/{_appState.CurrentLanguage}",
            async entry => {
                var message = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri(_contentLocation + _projectsSubPath + informalName + $"/page-content/{CultureToName(_appState.CurrentLanguage)}")
                ).SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);
                var response = await _httpClient.SendAsync(message);
                return await response.Content.ReadAsStringAsync();
            });

        return (await cached)!;
    }
}