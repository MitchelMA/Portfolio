using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Portfolio.Configuration;
using Portfolio.Model.Text;
using Portfolio.Services.CSV;

namespace Portfolio.Services;

public sealed class LanguageTable
{
    private const string LocationBase = "./Text";
    private static readonly string ManifestFileName = Path.Combine(LocationBase, "Language-tables_Manifest.json");
    private bool _isLoaded;
    private readonly NavigationManager _navManager;
    private readonly AppState _appState;
    private readonly HttpClient _httpClient;

    private static readonly CsvSettings _csvSettings = new()
    {
        FirstIsHeader = true,
        Separator = ',',
        CommentStarter = '#',
        Patches = false
    };

    private LanguageTableManifestModel _manifestContent;

    public IReadOnlyList<string> SupportedFileNames => _manifestContent.LanguageIndexTable;
    public bool IsLoaded => _isLoaded;
    
    public delegate void ManifestLoadedDelegate(object sender);
    public delegate Task ManifestLoadedDelegateAsync(object sender);
    public event ManifestLoadedDelegate? ManifestLoaded;
    public event ManifestLoadedDelegateAsync? ManifestLoadedAsync;

    public LanguageTable(NavigationManager navMan, AppState appState)
    {
        _navManager = navMan;
        _appState = appState;
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
        await OnManifestLoaded();
    }

    private async Task OnManifestLoaded()
    {
        _isLoaded = true;
        ManifestLoaded?.Invoke(this);
        await ManifestLoadedAsync?.Invoke(this)!;
    }

    public async Task<LangPageData?> LoadCurrentPageData()
    {
        if (!_isLoaded)
            return default;

        var langIdx = _appState.CurrentLanguage;
        var curUri = _navManager.ToBaseRelativePath(_navManager.Uri);
        curUri = curUri == string.Empty ? "index" : curUri;
        var headerFilepath = Path.Combine(LocationBase, curUri, _manifestContent.HeaderFileName);
        var linksFilepath = Path.Combine(LocationBase, curUri, _manifestContent.LinkDataFileName);
        var contentsFilepath = Path.Combine(LocationBase, curUri,
            _manifestContent.PageContentsPrefix + _manifestContent.LanguageIndexTable[langIdx] +
            ".csv");

        string headerFileContent;
        string linksFileContent;
        string contentsFileContent;

        try
        {
            headerFileContent = await _httpClient.GetStringAsync(headerFilepath);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Something went wrong trying to read data for the current page: {e}");
            return default;
        }

        LangPageData data = new();

        using var headerFileLexer = new CsvLexer(headerFileContent, _csvSettings);
        var headerContentDeserialized = (await headerFileLexer.DeserializeAsync<LangHeaderModel>())[langIdx];

        data.HeaderData = headerContentDeserialized;


        return data;
    }
}