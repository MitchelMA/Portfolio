using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Shared.Layouts;

namespace Portfolio.Pages.Projects;

public partial class CsvLexing : ComponentBase, IDisposable
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
    private IMapper<LangHeaderModel, HeaderData>? HeaderMapper { get; init; }
    [Inject]
    private IMapper<LangLinkModel, NavLinkData>? LinkMapper { get; init; }
    
    
    private static ProjectDataModel? _model;
    private static NavLinkData[]? _links;
    private static PageIslandModel[]? _islandsData;

    protected override async Task OnInitializedAsync()
    {
        _model = await ProjectInfoGetter!.GetCorrespondingToUri();
        ParentLayout.Model = _model;
        
        AppState!.PageIcon = StaticData.DefaultPageIcon;
        AppState.ShowFooter = true;
        
        LanguageTable!.LanguageChangedAsync += OnLanguageChanged;
        await LanguageTable.AwaitLanguageContentAsync(SetLangData);
    }

    private Task OnLanguageChanged(object sender, int newCultureIdx) => SetLangData(sender);

    private async Task SetLangData(object? sender)
    {
        var currentData = await LanguageTable!.GetAllPageData("csv-lexer");
        if (currentData is null)
        {
            await Console.Error.WriteLineAsync("Couldn't get Page Data in specified language!");
            return;
        }
        
        SetPageContent(currentData);
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
        _links = new NavLinkData[linksData.Links.Length];
        for (var i = 0; i < linksData.Links.Length; i++)
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