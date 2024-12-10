using Microsoft.AspNetCore.Components;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Tags;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Services.Markdown;
using Portfolio.Shared.Layouts;

namespace Portfolio.Pages.Projects;

public partial class LineCollisions : ComponentBase, IDisposable
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
    
    
    private static ProjectDataModel? _model;
    private static NavLinkData[]? _links;
    private static PageIslandModel[]? _islandsData;
    private string? _markdownText;

    private readonly ProjectMarkdown _projectMarkdown = new() {ExtraMode = true};

    protected override async Task OnInitializedAsync()
    {
        PreCacher!.Extra = new[] { "./index" };
        
        _model = await ProjectInfoGetter!.GetCorrespondingToUri();
        ParentLayout.Model = _model;

        // page details
        AppState!.PageIcon = new PageIcon("image/png", "./images/Unity_AmCJ2pY4oC.png");
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
        SetMarkdownData(langPageData.PageMarkdownText);
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

    private void SetMarkdownData(string? markdownText)
    {
        _markdownText = markdownText;
        StateHasChanged();
    }

    public void Dispose()
    {
        LanguageTable!.ManifestLoadedAsync -= SetLangData;
        LanguageTable.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}