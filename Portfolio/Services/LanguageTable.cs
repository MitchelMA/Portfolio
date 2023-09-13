using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Configuration;
using Portfolio.Extensions;
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

    #region CacheData contents
    // A nested Dictionary where the outer is for the page uri and the inner for the specified language:
    // _cachedData[RelativeUriString][LanguageCode]
    private readonly Dictionary<string, Dictionary<int, LangPageData>> _pageContentCachedData = new();
    
    // Cached text-contents for the page-info-table headers 
    private readonly Dictionary<int, InfoTableHeaderModel> _pageInfoTableCachedData = new();
    
    // Cached text-contents for the duration-type texts
    private readonly Dictionary<int, DurationTextsModel> _durationTextsCachedData = new();
    
    #endregion

    private static readonly CsvSettings CsvSettings = new()
    {
        FirstIsHeader = true,
        Separator = ',',
        CommentStarter = '#',
        Patches = true
    };

    private LanguageTableManifestModel _manifestContent;

    private string CurrentRelUri => _navManager.ToBaseRelativePath(_navManager.Uri);
    public IReadOnlyList<string> SupportedCultures => _manifestContent.LanguageIndexTable;
    public bool IsLoaded => _isLoaded;
    
    public delegate void ManifestLoadedDelegate(object sender);
    public delegate Task ManifestLoadedDelegateAsync(object sender);
    public delegate void LanguageChangedDelegate(object sender, int newCultureIdx);
    public delegate Task LanguageChangedDelegateAsync(object sender, int newCultureIdx);
    public event ManifestLoadedDelegate? ManifestLoaded;
    public event ManifestLoadedDelegateAsync? ManifestLoadedAsync;
    public event LanguageChangedDelegate? LanguageChanged;
    public event LanguageChangedDelegateAsync? LanguageChangedAsync;

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
    
    #region Event Callers

    private async Task OnManifestLoaded()
    {
        _isLoaded = true;
        ManifestLoaded?.Invoke(this);
        await ManifestLoadedAsync?.Invoke(this)!;
    }

    private async Task OnLanguageChanged(int newIdx)
    {
        _appState.CurrentLanguage = newIdx;
        LanguageChanged?.Invoke(this, newIdx);
        await LanguageChangedAsync?.Invoke(this, newIdx)!;
    }
    
    #endregion

    private string UriEscaper(string relativeUri)
    {
        if (relativeUri.StartsWith("./")) return relativeUri;
        return "./" + relativeUri;
    }
    
    #region Cache Exist Checkers

    private (bool exists, LangHeaderModel? langData) HeaderCacheExists(string relativeUri, int langCode)
    {
        relativeUri = UriEscaper(relativeUri);
        if (!_pageContentCachedData.TryGetValue(relativeUri, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.HeaderData is not null, langData.HeaderData);
    }

    private (bool exists, LangLinksModel? langData) LinksCacheExists(string relativeUri, int langCode)
    {
        relativeUri = UriEscaper(relativeUri);
        if (!_pageContentCachedData.TryGetValue(relativeUri, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.LinksData is not null, langData.LinksData);
    }

    private (bool exists, PageIslandModel[]? langData) PageIslandsCacheExists(string relativeUri, int langCode)
    {
        relativeUri = UriEscaper(relativeUri);
        if (!_pageContentCachedData.TryGetValue(relativeUri, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.PageIslandsData is not null, langData.PageIslandsData);
    }

    private (bool exits, InfoTableHeaderModel? langData) InfoTableCacheExists(int langCode)
    {
        if (!_pageInfoTableCachedData.TryGetValue(langCode, out var cache)) return (false, default);
        return (true, cache);

    }

    private (bool exits, DurationTextsModel? langData) DurationTextsCacheExists(int langCode)
    {
        if (!_durationTextsCachedData.TryGetValue(langCode, out var cache)) return (false, default);
        return (true, cache);
    }
    
    #endregion
    
    #region Cache Getters

    private Dictionary<int, LangPageData> GetPageSpecificCache(string relativeUri)
    {
        relativeUri = UriEscaper(relativeUri);
        if (_pageContentCachedData.TryGetValue(relativeUri, out var pageCache)) return pageCache;
        
        pageCache = new Dictionary<int, LangPageData>();
        _pageContentCachedData[relativeUri] = pageCache;

        return pageCache;
    }
    
    #endregion
    
    #region Cache Setters

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

    private bool CachePageIslandsDataForPage(string relativeUri, int langCode, PageIslandModel[] islandsContent)
    {
        relativeUri = UriEscaper(relativeUri);
        if (PageIslandsCacheExists(relativeUri, langCode).exists) return false;

        var pageData = GetPageSpecificCache(relativeUri);
        if (pageData.TryGetValue(langCode, out var langData))
        {
            langData.PageIslandsData = islandsContent;
        }
        else
        {
            pageData.TryAdd(langCode, new LangPageData { PageIslandsData = islandsContent });
        }

        return true;
    }

    private bool CacheInfoTable(IReadOnlyList<InfoTableHeaderModel> infoTableContent)
    {
        // all info-table text contents are in one file. If lang-code 0 exists, all the other should as well.
        if (InfoTableCacheExists(0).exits) return false;

        for (var langCode = 0; langCode < infoTableContent.Count; langCode++)
            _pageInfoTableCachedData.TryAdd(langCode, infoTableContent[langCode]);

        return true;
    }

    private bool CacheDurationTexts(IReadOnlyList<DurationTextsModel> durationTextsContent)
    {
        // all duration-type text contents are in one file. If lang-code 0 exists, all the other should as well.
        if (DurationTextsCacheExists(0).exits) return false;

        for (var langCode = 0; langCode < durationTextsContent.Count; langCode++)
            _durationTextsCachedData.TryAdd(langCode, durationTextsContent[langCode]);

        return true;
    }
    
    #endregion

    #region Language Content Getters
    
    public async Task<LangHeaderModel?> LoadHeaderForPage(string relativeUri, int langCode)
    {
        if (!_isLoaded)
            return default;
        
        relativeUri = UriEscaper(relativeUri);

        var (exists, langHeaderData) = HeaderCacheExists(relativeUri, langCode);
        if (exists)
            return langHeaderData;
        
        var headerFilePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, relativeUri, _manifestContent.HeaderFileName);
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

        var (exists, langLinksData) = LinksCacheExists(relativeUri, langCode);
        if (exists)
            return langLinksData;

        var linksFilePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, relativeUri, _manifestContent.LinkDataFileName);
        var linksContentDeserialized = await _httpClient.GetFromJsonAsync<LangLinksModel[]>(linksFilePath);

        CacheLinksDataForPage(relativeUri, linksContentDeserialized!);

        if (langCode >= linksContentDeserialized!.Length)
            return default;

        return linksContentDeserialized[langCode];
    }

    public async Task<PageIslandModel[]?> LoadIslandsForPage(string relativeUri, int langCode)
    {
        if (!_isLoaded)
            return default;

        relativeUri = UriEscaper(relativeUri);
        

        var (exists, langIslandData) = PageIslandsCacheExists(relativeUri, langCode);
        if (exists)
            return langIslandData;

        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, 
            relativeUri, _manifestContent.PageContentsPrefix + SupportedCultures[langCode] + ".json");
        
        PageIslandModel[]? contentDeserialized = null;
        try
        {
            contentDeserialized = await _httpClient.GetFromJsonAsync<PageIslandModel[]>(filePath);
        }
        catch(Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to deserialize content from island-json file: {e}");
        }

        if (contentDeserialized is null)
            return default;

        CachePageIslandsDataForPage(relativeUri, langCode, contentDeserialized);

        return contentDeserialized;
    }

    public async Task<InfoTableHeaderModel?> LoadInfoTableHeaderData(int langCode)
    {
        if (!_isLoaded)
            return default;

        var (exists, langData) = InfoTableCacheExists(langCode);
        if (exists)
            return langData;

        var filePath = Path.Combine(LocationBase, _manifestContent.OtherContentDirName,
            _manifestContent.InfoTableFileName);
        var fileContent = await _httpClient.GetStringAsync(filePath);

        using var fileLexer = new CsvLexer(fileContent, CsvSettings);
        var contentDeserialized = await fileLexer.DeserializeAsync<InfoTableHeaderModel>();

        CacheInfoTable(contentDeserialized);

        if (langCode >= contentDeserialized.Length)
            return default;

        return contentDeserialized[langCode];
    }

    public async Task<DurationTextsModel?> LoadDurationTexts(int langCode)
    {
        if (!_isLoaded)
            return default;

        var (exists, langData) = DurationTextsCacheExists(langCode);
        if (exists)
            return langData;

        var filePath = Path.Combine(LocationBase, _manifestContent.OtherContentDirName,
            _manifestContent.DurationTypeFilename);
        var content = await _httpClient.GetFromJsonAsync<DurationTextsModel[]>(filePath);

        CacheDurationTexts(content!);

        if (langCode >= content!.Length)
            return default;

        return content[langCode];
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

    public async Task<PageIslandModel[]?> LoadCurrentIslandsData()
    {
        var langCode = _appState.CurrentLanguage;
        var curUri = CurrentRelUri == string.Empty ? DefaultEmptyUri : CurrentRelUri;

        return await LoadIslandsForPage(curUri, langCode);
    }

    public async Task<InfoTableHeaderModel?> LoadCurrentInfoTableData()
    {
        var langCode = _appState.CurrentLanguage;
        return await LoadInfoTableHeaderData(langCode);
    }

    public async Task<DurationTextsModel?> LoadCurrentDurationTexts()
    {
        var langCode = _appState.CurrentLanguage;
        return await LoadDurationTexts(langCode);
    }

    public async Task<LangPageData?> LoadAllCurrentPageData()
    {
        if (!_isLoaded)
            return default;

        var headerData = await LoadCurrentHeaderData();
        var linksData = await LoadCurrentLinksData();
        var islandsData = await LoadCurrentIslandsData();
        if (headerData is null || linksData is null || islandsData is null)
            return default;

        LangPageData data = new()
        {
            HeaderData = headerData.Value,
            LinksData = linksData.Value,
            PageIslandsData = islandsData,
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
    
    #endregion

    #region Language Setters
    
    public bool SetLanguage(string cultureName)
    {
        var targetCulture = CultureInfo.GetCultureInfo(cultureName);
        var supportedCultures = SupportedCultures.Select(CultureInfo.GetCultureInfo);
        var nextLang = StaticData.DefaultLangCode;

        var idx = supportedCultures.BestGreedyEquivalent(targetCulture);
        if (idx != -1) nextLang = idx;
        if (nextLang == _appState.CurrentLanguage)
            return false;
        
        Task.Run(async () => await OnLanguageChanged(nextLang));
        return true;
    }

    public bool SetLanguage(int cultureIdx)
    {
        if (cultureIdx < 0 || cultureIdx >= SupportedCultures.Count)
            return false;

        if (cultureIdx == _appState.CurrentLanguage)
            return false;

        Task.Run(async () => await OnLanguageChanged(cultureIdx));
        return true;
    }

    public async Task<bool> SetLanguageAsync(string cultureName)
    {
        var targetCulture = CultureInfo.GetCultureInfo(cultureName);
        var supportedCultures = SupportedCultures.Select(CultureInfo.GetCultureInfo);
        var nextLang = StaticData.DefaultLangCode;

        var idx = supportedCultures.BestGreedyEquivalent(targetCulture);
        if (idx != -1) nextLang = idx;
        if (nextLang == _appState.CurrentLanguage)
            return false;
        
        await OnLanguageChanged(nextLang);
        return true;
    }

    public async Task<bool> SetLanguageAsync(int cultureIdx)
    {
        if (cultureIdx < 0 || cultureIdx >= SupportedCultures.Count)
            return false;

        if (cultureIdx == _appState.CurrentLanguage)
            return false;

        await OnLanguageChanged(cultureIdx);
        return true;
    }
    
    #endregion
}