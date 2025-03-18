using Microsoft.AspNetCore.Components;
using Portfolio.Client;
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
    private ContentProvider Provider { get; init; } = null!;
    [Inject]
    private HeroInfoGetter HeroInfoGetter { get; init; } = null!;

    [Parameter]
    public string HeroName { get; init; } = "";
    
    [CascadingParameter]
    public HeroLayout HeroLayout { get; init; } = null!;

    private NewProjectModel[]? _heroProjectsData;
    private NewHeroMeta? _heroData;
    private NewProjectMetaDataModel[]? _langHeaderData;

    protected override async Task OnInitializedAsync()
    {
        await Provider.AwaitSupportedLanguages(OnLanguageManifestLoadedAsync);
        
        LangTable.LanguageChangedAsync += OnLanguageChangedAsync;
    }

    private async Task OnLanguageManifestLoadedAsync()
    {
        await SetHeroData();
        await SetHeroProjectsData();
        await SetLanguageData();
    }

    private async Task OnLanguageChangedAsync(object sender, int newLanguage)
    {
        await SetHeroData();
        await SetHeroProjectsData();
        await SetLanguageData();
    }

    private async Task SetHeroData()
    {
        _heroData = (await Provider.GetHeroData(HeroName)).LocalInfo;
        HeroLayout.HeroInfo = _heroData!.Value;
        AppState.PageIcon = _heroData.Value.PageIcon is not null ? new PageIcon("image/icon", _heroData!.Value.PageIcon) : StaticData.DefaultPageIcon;
        AppState.PageTitleExtension = " - " + _heroData.Value.Title;
        StateHasChanged();
    }

    private async Task SetHeroProjectsData()
    {
        _heroProjectsData = await Provider.GetHeroProjects(HeroName);
        StateHasChanged();
    }

    private async Task SetLanguageData()
    {
        var length = _heroProjectsData!.Length;
        var projectMetaRequests = new Task<NewProjectMetaDataModel>[length];
        for (int i = 0; i < length; i++)
            projectMetaRequests[i] = Provider.GetProjectMeta(_heroProjectsData[i].InformalName);
        await Task.WhenAll(projectMetaRequests);

        _langHeaderData = projectMetaRequests.Select(x => x.Result).ToArray();
        
        StateHasChanged();
    }

    public void Dispose()
    {
        Provider.OnSupportedLanguagesLoadedAsync -= OnLanguageManifestLoadedAsync;
        LangTable.LanguageChangedAsync -= OnLanguageChangedAsync;
    }
}