using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Client;
using Portfolio.Extensions;
using Portfolio.Model.Hero;
using Portfolio.Model.Text;

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

    #region Language Content Getters

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
        return await GetHeroPageInfoCached(heroName, langCode);
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

    public async Task<HeroPageInfo?> GetHeroPageInfoCached(string heroName, int langCode)
    {
        if (!_isLoaded)
            return null;

        var outcome = await _contentCache.GetOrCreateAsync($"hero-content/{heroName}",
            async entry =>
            {
                var heroPageFilePath = Path.Combine(LocationBase, _manifestContent.HeroContentDirName, 
                    _manifestContent.HeroContentsPrefix + heroName + ".json");

                return await _httpClient.GetFromJsonAsync<HeroPageInfo[]>(heroPageFilePath);
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