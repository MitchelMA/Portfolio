using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Configuration;
using Portfolio.Converters.CSV;
using Portfolio.Deserialization;
using Portfolio.Extensions;
using Portfolio.Model.Hero;
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

    // A nested Dictionary where the outer is for the page uri and the inner for the specified language:
    // _cachedData[RelativeUriString][LanguageCode]
    // private readonly Dictionary<string, Dictionary<int, string>> _pageContentCachedMdData = new();

    // Cached text-contents for the page-info-table headers 
    private readonly Dictionary<int, InfoTableHeaderModel> _pageInfoTableCachedData = new();

    // Cached text-contents for the duration-type texts
    private readonly Dictionary<int, DurationTextsModel> _durationTextsCachedData = new();
    
    // A nested Dictionary where the outer is for the name of the hero and the inner is the index of the specified language:
    // _heroPageInfoCachedData[HeroName][LanguageCode]
    private readonly Dictionary<string, Dictionary<int, HeroPageInfo>> _heroPageInfoCachedData = new();

    #endregion

    private static readonly CsvSettings CsvSettings = new()
    {
        FirstIsHeader = true,
        Separator = ',',
        CommentStarter = '#',
        Patches = true
    };

    private LanguageTableManifestModel _manifestContent;

    private string CurrentRelUri
    {
        get
        {
            var basePath = UriEscaper(_navManager.ToBaseRelativePath(_navManager.Uri));
            if (basePath == "./") return "./" + DefaultEmptyUri;
            return basePath;
        }
    }

    public IReadOnlyList<string> SupportedCultures => _manifestContent.LanguageIndexTable;
    public bool IsLoaded => _isLoaded;

    public delegate void ManifestLoadedDelegate(object sender);

    public delegate Task ManifestLoadedDelegateAsync(object sender);

    public delegate void LanguageChangingDelegate(object? sender, int oldCultureIdx, int newCultureIdx);

    public delegate Task LanguageChangingDelegateAsync(object? sender, int oldCultureIdx, int newCultureIdx);

    public delegate void LanguageChangedDelegate(object sender, int newCultureIdx);

    public delegate Task LanguageChangedDelegateAsync(object sender, int newCultureIdx);

    public event ManifestLoadedDelegate? ManifestLoaded;
    public event ManifestLoadedDelegateAsync? ManifestLoadedAsync;
    public event LanguageChangingDelegate? LanguageChanging;
    public event LanguageChangingDelegateAsync? LanguageChangingAsync;
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
        OnManifestLoaded();
    }

    #region Event Callers

    private void OnManifestLoaded()
    {
        _isLoaded = true;
        ManifestLoaded?.Invoke(this);
        ManifestLoadedAsync?.Invoke(this);
    }

    private void OnLanguageChanging(int newIdx)
    {
        LanguageChanging?.Invoke(this, _appState.CurrentLanguage, newIdx);
        LanguageChangingAsync?.Invoke(this, _appState.CurrentLanguage, newIdx);
        OnLanguageChanged(newIdx);
    }

    private void OnLanguageChanged(int newIdx)
    {
        _appState.CurrentLanguage = newIdx;
        LanguageChanged?.Invoke(this, newIdx);
        LanguageChangedAsync?.Invoke(this, newIdx);
    }

    #endregion

    private string UriEscaper(string relativeUri)
    {
        var text = relativeUri.Split('?', '#')[0];
        if (text.StartsWith("./")) return text;
        return "./" + text;
    }

    #region Cache Exist Checkers

    private (bool exists, LangHeaderModel? langData) HeaderCacheExists(string informalName, int langCode)
    {
        if (!_pageContentCachedData.TryGetValue(informalName, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.HeaderData is not null, langData.HeaderData);
    }

    private (bool exists, LangLinksModel? langData) LinksCacheExists(string informalName, int langCode)
    {
        if (!_pageContentCachedData.TryGetValue(informalName, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.LinksData is not null, langData.LinksData);
    }

    private (bool exists, PageIslandModel[]? langData) PageIslandsCacheExists(string informalName, int langCode)
    {
        if (!_pageContentCachedData.TryGetValue(informalName, out var pageCache)) return (false, default);
        if (!pageCache.TryGetValue(langCode, out var langData)) return (false, default);
        return (langData.PageIslandsData is not null, langData.PageIslandsData);
    }

    private string? PageMarkdownCacheExists(string informalName, int langCode)
    {
        if (!_pageContentCachedData.TryGetValue(informalName, out var pageCache)) return null;
        if (!pageCache.TryGetValue(langCode, out var langPageData)) return null;
        return langPageData.PageMarkdownText;
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

    private (bool exists, HeroPageInfo? langData) HeroPageInfoCacheExists(string heroName, int langCode)
    {
        if (!_heroPageInfoCachedData.TryGetValue(heroName, out var cache)) return (false, default);
        if (!cache.TryGetValue(langCode, out var langData)) return (false, default);
        return (true, langData);
    }
    
    #endregion

    #region Cache Getters

    private Dictionary<int, LangPageData> GetPageSpecificCache(string informalName)
    {
        if (_pageContentCachedData.TryGetValue(informalName, out var pageCache)) return pageCache;

        pageCache = new Dictionary<int, LangPageData>();
        _pageContentCachedData[informalName] = pageCache;

        return pageCache;
    }

    private Dictionary<int, HeroPageInfo> GetHeroSpecificCache(string heroName)
    {
        if (_heroPageInfoCachedData.TryGetValue(heroName, out var cache)) return cache;
        
        cache = new Dictionary<int, HeroPageInfo>();
        _heroPageInfoCachedData[heroName] = cache;

        return cache;
    }

    #endregion

    #region Cache Setters

    private bool CacheHeaderDataForPage(string informalName, IReadOnlyList<LangHeaderModel> headerContent)
    {
        // All header-content is saved in one file. If lang-code 0 exists, all the other should as well.
        if (HeaderCacheExists(informalName, 0).exists) return false;

        for (var langCode = 0; langCode < headerContent.Count; langCode++)
        {
            var pageData = GetPageSpecificCache(informalName);
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

    private bool CacheLinksDataForPage(string informalName, IReadOnlyList<LangLinksModel> linksContent)
    {
        // all links-content is saved in one file. If lang-code 0 exists, all the other should as well.
        if (LinksCacheExists(informalName, 0).exists) return false;

        for (var langcode = 0; langcode < linksContent.Count; langcode++)
        {
            var pageData = GetPageSpecificCache(informalName);
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

    private bool CachePageIslandsDataForPage(string informalName, int langCode, PageIslandModel[] islandsContent)
    {
        if (PageIslandsCacheExists(informalName, langCode).exists) return false;

        var pageData = GetPageSpecificCache(informalName);
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

    private bool CacheMarkdownForPage(string informalName, int langCode, string markdownText)
    {
        if (PageMarkdownCacheExists(informalName, langCode) is not null) return false;
        var pageData = GetPageSpecificCache(informalName);
        if (pageData.TryGetValue(langCode, out var langData))
        {
            langData.PageMarkdownText = markdownText;
        }
        else
        {
            pageData.TryAdd(langCode, new LangPageData {PageMarkdownText = markdownText});
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

    private bool CacheHeroData(string heroName, IReadOnlyList<HeroPageInfo> pageInfo)
    {
        if (HeroPageInfoCacheExists(heroName, 0).exists) return false;
        
        for (var langCode = 0; langCode < pageInfo.Count; langCode++)
        {
            var heroData = GetHeroSpecificCache(heroName);
            if (heroData.TryGetValue(langCode, out var langData))
            {
                heroData[langCode] = pageInfo[langCode];
            }
            else
            {
                heroData.TryAdd(langCode, pageInfo[langCode]);
            }
        }

        return true;
    }
    
    #endregion

    #region Language Content Getters

    public async Task<LangHeaderModel?> LoadHeaderForPage(string informalName, int langCode)
    {
        if (!_isLoaded)
            return default;

        var (exists, langHeaderData) = HeaderCacheExists(informalName, langCode);
        if (exists)
            return langHeaderData;

        var headerFilePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, informalName,
            _manifestContent.HeaderFileName);
        var headerFileContent = await _httpClient.GetStringAsync(headerFilePath);

        using var headerFileLexer = new CsvLexer(headerFileContent, CsvSettings);
        var headerContentDeserialized = await headerFileLexer.DeserializeAsync<LangHeaderModel>();

        CacheHeaderDataForPage(informalName, headerContentDeserialized);

        if (langCode >= headerContentDeserialized.Length)
            return default;

        return headerContentDeserialized[langCode];
    }

    public async Task<LangLinksModel?> LoadLinksForPage(string informalName, int langCode)
    {
        if (!_isLoaded)
            return default;

        var (exists, langLinksData) = LinksCacheExists(informalName, langCode);
        if (exists)
            return langLinksData;

        var linksFilePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, informalName,
            _manifestContent.LinkDataFileName);
        var linksContentDeserialized = await _httpClient.GetFromJsonAsync<LangLinksModel[]>(linksFilePath);

        CacheLinksDataForPage(informalName, linksContentDeserialized!);

        if (langCode >= linksContentDeserialized!.Length)
            return default;

        return linksContentDeserialized[langCode];
    }

    public async Task<PageIslandModel[]?> LoadIslandsForPage(string informalName, int langCode)
    {
        if (!_isLoaded)
            return default;

        var (exists, langIslandData) = PageIslandsCacheExists(informalName, langCode);
        if (exists)
            return langIslandData;

        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
            informalName, _manifestContent.PageContentsPrefix + SupportedCultures[langCode] + ".json");

        PageIslandModel[]? contentDeserialized;
        try
        {
            contentDeserialized = await _httpClient.GetFromJsonAsync<PageIslandModel[]>(filePath);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to deserialize content from island-json file: {e}");
            return default;
        }

        if (contentDeserialized is null)
            return default;

        var modelCount = contentDeserialized.Length;
        for (var i = 0; i < modelCount; i++)
        {
            var itemPath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
                informalName, _manifestContent.PageIslandsTextLocation, SupportedCultures[langCode], $"_{i}.html");
            contentDeserialized[i].HtmlContentString = await _httpClient.GetStringAsync(itemPath);
        }

        CachePageIslandsDataForPage(informalName, langCode, contentDeserialized);

        return contentDeserialized;
    }

    public async Task<string?> LoadMarkdownForPage(string informalName, int langCode)
    {
        if (!_isLoaded)
            return default;

        var markdownCache = PageMarkdownCacheExists(informalName, langCode);
        if (markdownCache is not null)
            return markdownCache;

        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName, informalName,
            $"text_{SupportedCultures[langCode]}.md");

        try
        {
            var markdownText = await _httpClient.GetStringAsync(filePath);
            CacheMarkdownForPage(informalName, langCode, markdownText);
            return markdownText;
        }
        catch
        {
            // await Console.Error.WriteLineAsync($"Failed to retrieve page-content from file ${filePath}");
        }

        return null;
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

    public async Task<HeroPageInfo?> LoadHeroPageInfo(string heroName, int langCode)
    {
        if (!_isLoaded)
            return default;
        
        var (exists, langData) = HeroPageInfoCacheExists(heroName, langCode);
        if (exists)
            return langData;

        var heroPageFilePath = Path.Combine(LocationBase, _manifestContent.HeroContentDirName, 
            _manifestContent.HeroContentsPrefix + heroName + ".csv");
        var fileContent = await _httpClient.GetStringAsync(heroPageFilePath);
        
        using var heroPageLexer = new CsvLexer(fileContent, CsvSettings);
        CsvDeserializationOptions deserializationOptions = new(
            new List<ICsvConverter>
            {
                new CsvColorConverter()
            }
        );
        var heroContentDeserialized = await heroPageLexer.DeserializeAsync<HeroPageInfo>(deserializationOptions);

        CacheHeroData(heroName, heroContentDeserialized);

        if (langCode >= heroContentDeserialized.Length)
            return default;
        
        return heroContentDeserialized[langCode];

    }
    
    
    private Task<LangHeaderModel?> LoadCurrentHeaderData()
    {
        var langCode = _appState.CurrentLanguage;
        var informalName = CurrentRelUri.Split('/').Last();

        return LoadHeaderForPage(informalName, langCode);
    }

    private Task<LangLinksModel?> LoadCurrentLinksData()
    {
        var langCode = _appState.CurrentLanguage;
        var informalName = CurrentRelUri.Split('/').Last();

        return LoadLinksForPage(informalName, langCode);
    }

    private Task<PageIslandModel[]?> LoadCurrentIslandsData()
    {
        var langCode = _appState.CurrentLanguage;
        var informalName = CurrentRelUri.Split('/').Last();

        return LoadIslandsForPage(informalName, langCode);
    }

    private Task<string?> LoadCurrentMarkdownData()
    {
        var langCode = _appState.CurrentLanguage;
        var informalName = CurrentRelUri.Split('/').Last();

        return LoadMarkdownForPage(informalName, langCode);
    }

    public Task<InfoTableHeaderModel?> LoadCurrentInfoTableData()
    {
        var langCode = _appState.CurrentLanguage;
        return LoadInfoTableHeaderData(langCode);
    }

    public Task<DurationTextsModel?> LoadCurrentDurationTexts()
    {
        var langCode = _appState.CurrentLanguage;
        return LoadDurationTexts(langCode);
    }

    public Task<HeroPageInfo?> LoadCurrentHeroPageInfo(string heroName)
    {
        var langCode = _appState.CurrentLanguage;
        return LoadHeroPageInfo(heroName, langCode);
    }

    public async Task<LangPageData?> LoadAllCurrentPageData(bool asMarkdown = true)
    {
        if (!_isLoaded)
            return default;
        
        var headerData = LoadCurrentHeaderData();
        var linksData = LoadCurrentLinksData();

        var markdownData = Task.FromResult<string?>(null);
        var islandsData = Task.FromResult<PageIslandModel[]?>(null);
        if (asMarkdown)
        {
            markdownData = LoadCurrentMarkdownData();
        }
        else
        {
            islandsData = LoadCurrentIslandsData();
        }

        LangPageData data = new()
        {
            HeaderData = await headerData,
            LinksData = await linksData,
            PageIslandsData = await islandsData,
            PageMarkdownText = await markdownData
        };

        return data;
    }

    public bool PreCacheAll(IEnumerable<string> informalNames, int langCode)
    {
        if (!_isLoaded)
            return false;

        var enumerated = informalNames as string[] ?? informalNames.ToArray();

        var uriCount = enumerated.Length;
        for (var i = 0; i < uriCount; i++)
        {
            var currentInformalName = enumerated[i];
            _ = LoadHeaderForPage(currentInformalName, langCode);
            _ = LoadLinksForPage(currentInformalName, langCode);
            _ = LoadMarkdownForPage(currentInformalName, langCode);
            // We're not pre-caching page island data, as that is an outdated system I used.
            // Only the landing-page uses this and thus it needs not to be pre-fetched
        }

        return true;
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

    public int GetLanguageIdx(string cultureName)
    {
        var targetCulture = CultureInfo.GetCultureInfo(cultureName);
        var supportedCultures = SupportedCultures.Select(CultureInfo.GetCultureInfo);
        var langIdx = StaticData.DefaultLangCode;

        var best = supportedCultures.BestGreedyEquivalent(targetCulture);
        if (best != -1) langIdx = best;
        return langIdx;
    }

    public bool SetLanguage(string cultureName)
    {
        var nextLang = GetLanguageIdx(cultureName);
        if (nextLang == _appState.CurrentLanguage)
            return false;

        OnLanguageChanging(nextLang);
        return true;
    }

    public bool SetLanguage(int cultureIdx)
    {
        if (cultureIdx < 0 || cultureIdx >= SupportedCultures.Count)
            return false;

        if (cultureIdx == _appState.CurrentLanguage)
            return false;

        OnLanguageChanging(cultureIdx);
        return true;
    }

    #endregion
}