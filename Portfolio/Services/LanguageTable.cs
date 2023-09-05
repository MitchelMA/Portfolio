using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Portfolio.Model.Text;

namespace Portfolio.Services;

public sealed class LanguageTable
{
    private const string ManifestFileName = "./Text/Language-tables_Manifest.json";
    private bool _isLoaded;
    private readonly NavigationManager _navManager;
    private readonly HttpClient _httpClient;
    
    private LanguageTableManifestModel _manifestContent;

    public IReadOnlyList<string> SupportedFileNames => _manifestContent.LanguageIndexTable;
    
    public delegate void ManifestLoadedDelegate(object sender);
    public event ManifestLoadedDelegate? ManifestLoaded;
    

    public LanguageTable(NavigationManager navMan)
    {
        _navManager = navMan;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_navManager.BaseUri)
        };
        Task.Run(async () => await LoadManifest());
    }

    private async Task LoadManifest()
    {
        if (_isLoaded)
            return;
        _manifestContent = await _httpClient.GetFromJsonAsync<LanguageTableManifestModel>(ManifestFileName);
        OnManifestLoaded();
    }

    private void OnManifestLoaded()
    {
        _isLoaded = true;
        ManifestLoaded?.Invoke(this);
    }
}