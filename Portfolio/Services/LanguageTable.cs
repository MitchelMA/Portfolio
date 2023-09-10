using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Portfolio.Configuration;
using Portfolio.Model.Text;
using Portfolio.Services.CSV;

namespace Portfolio.Services;

public sealed class LanguageTable
{
    private const string DefaultEmptyUri = "index";
    private const string LocationBase = "./Text";
    private static readonly string ManifestFileName = Path.Combine(LocationBase, "Language-tables_Manifest.json");
    private bool _isLoaded;
    private readonly NavigationManager _navManager;
    private readonly AppState _appState;
    private readonly HttpClient _httpClient;

    // A nested Dictionary where the outer is for the page uri and the inner for the specified language:
    // _cachedData[RelativeUriString][LanguageCode]
    private readonly Dictionary<string, Dictionary<int, LangPageData>> _cachedData = new();

    private static readonly CsvSettings CsvSettings = new()
    {
        FirstIsHeader = true,
        Separator = ',',
        CommentStarter = '#',
        Patches = true
    };

    private LanguageTableManifestModel _manifestContent;

    private string CurrentRelUri => _navManager.ToBaseRelativePath(_navManager.Uri);
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

    private string UriEscaper(string relativeUri)
    {
        if (relativeUri.StartsWith("./")) return relativeUri;
        return "./" + relativeUri;
    }

    private (bool exists, LangHeaderModel? langData) HeaderCacheExists(string relativeUri, int langCode)
    {
        relativeUri = UriEscaper(relativeUri);
        if (!_cachedData.TryGetValue(relativeUri, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.HeaderData is not null, langData.HeaderData);
    }

    private (bool exists, LangLinksModel? langData) LinksCacheExists(string relativeUri, int langCode)
    {
        relativeUri = UriEscaper(relativeUri);
        if (!_cachedData.TryGetValue(relativeUri, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.LinksData is not null, langData.LinksData);
    }

    private Dictionary<int, LangPageData> GetPageSpecificCache(string relativeUri)
    {
        relativeUri = UriEscaper(relativeUri);
        if (_cachedData.TryGetValue(relativeUri, out var pageCache)) return pageCache;
        
        pageCache = new Dictionary<int, LangPageData>();
        _cachedData[relativeUri] = pageCache;

        return pageCache;
    }

    private bool CacheHeaderDataForPage(string relativeUri, IReadOnlyList<LangHeaderModel> headerContent)
    {
        relativeUri = UriEscaper(relativeUri);
        // All header-content is saved in one file. If lang-code 0 exists, all the other should as well.
        if (HeaderCacheExists(relativeUri, 0).exists) return false;

        for (var langCode = 0; langCode < headerContent.Count; langCode++)
        {
            var pageData = GetPageSpecificCache(relativeUri);
            if (pageData.TryGetValue(langCode, out var langData))
            {
                langData.HeaderData = headerContent[langCode];
            }
            else
            {
                pageData.TryAdd(langCode, new LangPageData { HeaderData = headerContent[langCode] });
            }

        }

        return true;
    }

    private bool CacheLinksDataForPage(string relativeUri, IReadOnlyList<LangLinksModel> linksContent)
    {
        relativeUri = UriEscaper(relativeUri);
        // all links-content is saved in one file. If lang-code 0 exists, all the other should as well.
        if (LinksCacheExists(relativeUri, 0).exists) return false;

        for (var langcode = 0; langcode < linksContent.Count; langcode++)
        {
            var pageData = GetPageSpecificCache(relativeUri);
            if (pageData.TryGetValue(langcode, out var langData))
            {
                langData.LinksData = linksContent[langcode];
            }
            else
            {
                pageData.TryAdd(langcode, new LangPageData { LinksData = linksContent[langcode] });
            }
        }

        return true;
    }

    public async Task<LangHeaderModel?> LoadHeaderForPage(string relativeUri, int langCode)
    {
        if (!_isLoaded)
            return default;
        
        relativeUri = UriEscaper(relativeUri);

        if (HeaderCacheExists(relativeUri, langCode).exists)
            return GetPageSpecificCache(relativeUri)[langCode].HeaderData;
        
        var headerFilePath = Path.Combine(LocationBase, relativeUri, _manifestContent.HeaderFileName);
        var headerFileContent = await _httpClient.GetStringAsync(headerFilePath);

        using var headerFileLexer = new CsvLexer(headerFileContent, CsvSettings);
        var headerContentDeserialized = await headerFileLexer.DeserializeAsync<LangHeaderModel>();

        CacheHeaderDataForPage(relativeUri, headerContentDeserialized);

        if (langCode >= headerContentDeserialized.Length)
            return default;

        return headerContentDeserialized[langCode];
    }

    public async Task<LangLinksModel?> LoadLinksForPage(string relativeUri, int langCode)
    {
        if (!_isLoaded)
            return default;
        
        relativeUri = UriEscaper(relativeUri);

        if (LinksCacheExists(relativeUri, langCode).exists)
            return GetPageSpecificCache(relativeUri)[langCode].LinksData;

        var linksFilePath = Path.Combine(LocationBase, relativeUri, _manifestContent.LinkDataFileName);
        var linksContentDeserialized = await _httpClient.GetFromJsonAsync<LangLinksModel[]>(linksFilePath);

        CacheLinksDataForPage(relativeUri, linksContentDeserialized!);

        if (langCode >= linksContentDeserialized!.Length)
            return default;

        return linksContentDeserialized[langCode];
    }

    public async Task<LangHeaderModel?> LoadCurrentHeaderData()
    {
        var langCode = _appState.CurrentLanguage;
        var curUri = CurrentRelUri == string.Empty ? DefaultEmptyUri : CurrentRelUri;

        return await LoadHeaderForPage(curUri, langCode);
    }

    public async Task<LangLinksModel?> LoadCurrentLinksData()
    {
        var langCode = _appState.CurrentLanguage;
        var curUri = CurrentRelUri == string.Empty ? DefaultEmptyUri : CurrentRelUri;

        return await LoadLinksForPage(curUri, langCode);
    }

    public async Task<LangPageData?> LoadAllCurrentPageData()
    {
        if (!_isLoaded)
            return default;

        var headerData = await LoadCurrentHeaderData();
        var linksData = await LoadCurrentLinksData();
        if (headerData is null || linksData is null)
            return default;

        LangPageData data = new()
        {
            HeaderData = headerData.Value,
            LinksData = linksData.Value
        };

        return data;
    }

    public void AwaitLanguageContent(ManifestLoadedDelegate onReady)
    {
        if (_isLoaded)
        {
            onReady.Invoke(this);
            return;
        }

        ManifestLoaded += onReady;
    }

    public async Task AwaitLanguageContentAsync(ManifestLoadedDelegateAsync onReady)
    {
        if (_isLoaded)
        {
            await onReady.Invoke(this);
            return;
        }

        ManifestLoadedAsync += onReady;

    }
}