using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Shared.Layouts;

namespace Portfolio.Pages.Projects;

public partial class VerticalSlice : ComponentBase, IDisposable
{
    [CascadingParameter]
    private ProjectLayout ParentLayout { get; init; } = null!;
    
    [Inject]
    private AppState? AppState { get; init; }
    [Inject]
    private ProjectInfoGetter? ProjectInfoGetter { get; init; }
    [Inject]
    private LanguageTable? LanguageTable { get; init; }
    [Inject]
    private LangTablePreCacher? PreCacher { get; init; }
    
    [Inject]
    private IMapper<LangHeaderModel, HeaderData>? HeaderMapper { get; init; }
    [Inject]
    private IMapper<LangLinkModel, NavLinkData>? LinkMapper { get; init; }
    
    private static NavLinkData[]? _links;
    private static ProjectDataModel? _model;
    private static PageIslandModel[]? _islandsData;

    protected override async Task OnInitializedAsync()
    {
        PreCacher!.Extra = new[] { "./index" };
        
        _model = await ProjectInfoGetter!.GetCorrespondingToUri();
        ParentLayout.Model = _model;

        // setup of page-details
        AppState!.PageIcon = StaticData.DefaultPageIcon;
        AppState.ShowFooter = true;

        LanguageTable!.LanguageChangedAsync += OnLanguageChanged;
        await LanguageTable.AwaitLanguageContentAsync(SetLangData);
    }
    
    private Task OnLanguageChanged(object sender, int newCultureIdx) => SetLangData(sender);

    private async Task SetLangData(object? sender)
    {
        var currentData = await LanguageTable!.LoadAllCurrentPageData();
        if (currentData is null)
        {
            await Console.Error.WriteLineAsync("Couldn't get Page Data in specified language!");
            return;
        }

        SetPageContent(currentData);
        await PreCacher!.PreCache(AppState!.CurrentLanguage);
    }

    private void SetPageContent(LangPageData langPageData)
    {
        SetHeaderData(langPageData.HeaderData!.Value);
        SetLinksData(langPageData.LinksData!.Value);
        SetIslandsData(langPageData.PageIslandsData!);
    }

    private void SetHeaderData(LangHeaderModel headerData)
    {
        var header = HeaderMapper!.MapFrom(headerData);
        header.ImagePath = _model!.Value.Header.HeaderImage;
        AppState!.HeaderData = header;
        AppState.PageTitleExtension = " " + headerData.PageTitleExtension;
    }

    private void SetLinksData(LangLinksModel linksData)
    {
        var l = linksData.Links.Length;
        _links = new NavLinkData[l];
        for (var i = 0; i < l; i++)
            _links[i] = LinkMapper!.MapFrom(linksData.Links[i]);

        AppState!.Links = _links;
        AppState.MinNonStackedSize = linksData.MinWidth;
    }

    private void SetIslandsData(PageIslandModel[] islandsData)
    {
        _islandsData = islandsData;
        StateHasChanged();
    }

    public void Dispose()
    {
        LanguageTable!.ManifestLoadedAsync -= SetLangData;
        LanguageTable.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}