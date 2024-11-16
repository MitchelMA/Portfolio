using Microsoft.AspNetCore.Components;
using Portfolio.Model.Hero;
using Portfolio.Model.Project;
using Portfolio.Services;

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

    private ProjectDataModel[]? _heroProjectsData;
    private HeroPageInfo? _heroData;

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
    }

    private async Task OnLanguageChangedAsync(object sender, int newLanguage)
    {
        await SetHeroData();
    }

    private async Task SetHeroData()
    {
        _heroData = await LangTable.LoadCurrentHeroPageInfo(HeroName);
        StateHasChanged();
    }

    private void SetHeroProjectsData()
    {
        _heroProjectsData = ProjectInfoGetter.Data.Values.Where(x => x.Heroes != null && x.Heroes.Contains(HeroName)).ToArray();
        StateHasChanged();
    }

    public void Dispose()
    {
        LangTable.ManifestLoadedAsync -= OnLanguageManifestLoadedAsync;
        LangTable.LanguageChangedAsync -= OnLanguageChangedAsync;
    }
}