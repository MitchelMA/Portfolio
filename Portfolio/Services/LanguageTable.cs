using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
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

    private IMemoryCache _contentCache;

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

    public LanguageTable(NavigationManager navMan, AppState appState, IMemoryCache contentCache)
    {
        _navManager = navMan;
        _appState = appState;
        _contentCache = contentCache;
        
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

    public async Task<InfoTableHeaderModel?> LoadCurrentInfoTableData()
    {
        var langCode = _appState.CurrentLanguage;
        return await GetInfoTableHeaderDataCached(langCode);
    }

    public async Task<DurationTextsModel?> LoadCurrentDurationTexts()
    {
        var langCode = _appState.CurrentLanguage;
        return await GetDurationTextsCached(langCode);
    }

    public async Task<HeroPageInfo?> LoadCurrentHeroPageInfo(string heroName)
    {
        var langCode = _appState.CurrentLanguage;
        return await LoadHeroPageInfo(heroName, langCode);
    }

    public async Task<LangPageData?> GetAllPageData(string informalName, bool asMarkdown = true)
    {
        if (!_isLoaded)
            return default;
        
        var headerData = GetPageMetaDataCached(informalName, _appState.CurrentLanguage);
        var linksData = GetPageLinks(informalName, _appState.CurrentLanguage);
        var markdownData = Task.FromResult<string?>(null);
        var islandsData = Task.FromResult<PageIslandModel[]?>(null);
        
        if (asMarkdown)
        {
            markdownData = GetPageMarkdownCached(informalName, _appState.CurrentLanguage);
        }
        else
        {
            islandsData = GetIslandDataCached(informalName, _appState.CurrentLanguage);
        }

        await Task.WhenAll(headerData, linksData, markdownData, islandsData);

        LangPageData data = new()
        {
            HeaderData = headerData.Result,
            LinksData = linksData.Result,
            PageIslandsData = islandsData.Result,
            PageMarkdownText = markdownData.Result
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

    #region New Cached Content Getters

    public async Task<LangHeaderModel?> GetPageMetaDataCached(string informalName, int langCode)
    {
        if (!_isLoaded)
            return null;
    
        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
            informalName, SupportedCultures[langCode], _manifestContent.HeaderFileName);
        
        return await _contentCache.GetOrCreateAsync<LangHeaderModel>($"meta-data/{informalName}/{langCode}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _httpClient.GetFromJsonAsync<LangHeaderModel>(filePath);
            });
    }

    public async Task<LangLinksModel?> GetPageLinks(string informalName, int langCode)
    {
        if (!_isLoaded)
            return null;

        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
            informalName, SupportedCultures[langCode], _manifestContent.LinkDataFileName);

        return await _contentCache.GetOrCreateAsync<LangLinksModel>($"links/{informalName}/{langCode}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _httpClient.GetFromJsonAsync<LangLinksModel>(filePath);
            });
    }

    public async Task<PageIslandModel[]?> GetIslandDataCached(string informalName, int langCode)
    {
        if (!_isLoaded)
            return null;
        
        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
            informalName, SupportedCultures[langCode], _manifestContent.PageContentsPrefix + ".json");

        return await _contentCache.GetOrCreateAsync($"page-islands/{informalName}/{langCode}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                
                var contentDeserialized = await _httpClient.GetFromJsonAsync<PageIslandModel[]>(filePath);
                var islandCount = contentDeserialized!.Length;

                var tasks = new Task<string>[islandCount];
                for (int i = 0; i < islandCount; i++)
                {
                    var itemPath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
                        informalName, SupportedCultures[langCode], _manifestContent.PageIslandsTextLocation, $"_{i}.html");
                    tasks[i] = _httpClient.GetStringAsync(itemPath);
                }

                await Task.WhenAll(tasks);

                for (int i = 0; i < islandCount; i++)
                    contentDeserialized[i].HtmlContentString = tasks[i].Result;

                return contentDeserialized;
            });
    }

    public async Task<string?> GetPageMarkdownCached(string informalName, int langCode)
    {
        if (!_isLoaded)
            return null;
        
        var filePath = Path.Combine(LocationBase, _manifestContent.PageContentDirName,
            informalName, SupportedCultures[langCode], "text.md");

        return await _contentCache.GetOrCreateAsync<string>($"page-markdown/{informalName}/{langCode}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _httpClient.GetStringAsync(filePath);
            });
    }

    public async Task<InfoTableHeaderModel?> GetInfoTableHeaderDataCached(int langCode)
    {
        if (!_isLoaded)
            return null;
        
        var filePath = Path.Combine(LocationBase, _manifestContent.OtherContentDirName,
            _manifestContent.InfoTableFileName);

        var outcome = await _contentCache.GetOrCreateAsync("info-table",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _httpClient.GetFromJsonAsync<InfoTableHeaderModel[]>(filePath);
            });

        return outcome![langCode];
    }

    public async Task<DurationTextsModel?> GetDurationTextsCached(int langCode)
    {
        if (!_isLoaded)
            return null;
        
        var filePath = Path.Combine(LocationBase, _manifestContent.OtherContentDirName,
            _manifestContent.DurationTypeFilename);

        var outcome = await _contentCache.GetOrCreateAsync("info-table/durations",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _httpClient.GetFromJsonAsync<DurationTextsModel[]>(filePath);
            });

        return outcome![langCode];
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