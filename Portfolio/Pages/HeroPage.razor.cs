using Microsoft.AspNetCore.Components;
using Portfolio.Model.Hero;
using Portfolio.Model.Project;
using Portfolio.Model.Tags;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Shared.Layouts;

namespace Portfolio.Pages;

public partial class HeroPage : ComponentBase, IDisposable
{
    [Inject]
    private AppState AppState { get; init; } = null!;
    [Inject]
    private LanguageTable LangTable { get; init; } = null!;
    [Inject]
    private ProjectInfoGetter ProjectInfoGetter { get; init; } = null!;
    [Inject]
    private HeroInfoGetter HeroInfoGetter { get; init; } = null!;

    [Parameter]
    public string HeroName { get; init; } = "";
    
    [CascadingParameter]
    public HeroLayout HeroLayout { get; init; } = null!;

    private ProjectDataModel[]? _heroProjectsData;
    private HeroPageInfo? _heroData;
    private LangHeaderModel[]? _langHeaderData;

    protected override async Task OnInitializedAsync()
    {
        await ProjectInfoGetter.RetrieveData();
        SetHeroProjectsData();
        
        await LangTable.AwaitLanguageContentAsync(OnLanguageManifestLoadedAsync);
        LangTable.LanguageChangedAsync += OnLanguageChangedAsync;
    }

    private async Task OnLanguageManifestLoadedAsync(object sender)
    {
        await SetHeroData();
        await SetLanguageData();
    }

    private async Task OnLanguageChangedAsync(object sender, int newLanguage)
    {
        await SetHeroData();
        await SetLanguageData();
    }

    private async Task SetHeroData()
    {
        _heroData = await LangTable.LoadCurrentHeroPageInfo(HeroName);
        HeroLayout.HeroInfo = _heroData;
        AppState.PageIcon = new PageIcon("image/icon", _heroData!.Value.PageIcon);
        AppState.PageTitleExtension = " " + _heroData.Value.PageTitleExtension;
        StateHasChanged();
    }

    private void SetHeroProjectsData()
    {
        _heroProjectsData = ProjectInfoGetter.Data.Values.Where(x => x.Heroes != null && x.Heroes.Contains(HeroName)).ToArray();
        StateHasChanged();
    }

    private async Task SetLanguageData()
    {
        var l = _heroProjectsData!.Length;
        var tasks = new Task<LangHeaderModel?>[l];
        
        for (int i = 0; i < l; i++)
            tasks[i] = LangTable.GetPageMetaDataCached(_heroProjectsData[i].InformalName, AppState.CurrentLanguage);
        await Task.WhenAll(tasks);

        _langHeaderData = new LangHeaderModel[l];
        for (int i = 0; i < l; i++)
            _langHeaderData[i] = tasks[i].Result!.Value;
        
        StateHasChanged();
    }

    public void Dispose()
    {
        LangTable.ManifestLoadedAsync -= OnLanguageManifestLoadedAsync;
        LangTable.LanguageChangedAsync -= OnLanguageChangedAsync;
    }
}