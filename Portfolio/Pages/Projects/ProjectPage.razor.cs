using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Services.Markdown;
using Portfolio.Shared.Components.Lightbox;
using Portfolio.Shared.Layouts;

namespace Portfolio.Pages.Projects;

public partial class ProjectPage : ComponentBase, IDisposable
{
    private static string _projectName = "";

    [Parameter]
    public string ProjectName
    {
        get => _projectName;
        init
        {
            if (value == _projectName)
                return;
            
            _projectName = value;
            Task.Run(async () => { await SetPageData(); });
        }
    }


    [CascadingParameter]
    private ProjectLayout ParentLayout { get; init; } = null!;
    
    [Inject]
    private AppState? AppState { get; init; }
    [Inject]
    private ProjectInfoGetter? ProjectInfoGetter { get; init; }
    [Inject]
    private LanguageTable? LanguageTable { get; init; }
    [Inject]
    private LangTablePreCacher? LangTablePreCacher { get; init; }
    
    [Inject]
    private IMapper<LangHeaderModel, HeaderData>? HeaderDataMapper { get; init; }
    [Inject]
    private IMapper<LangLinkModel, NavLinkData>? NavLinkDataMapper { get; init; }

    private ImageEnlargeContainer? EnlargeContainer { get; set; }
    
    private NavLinkData[]? _links;
    private ProjectDataModel? _model;
    private string? _markdownText;
    
    private readonly ProjectMarkdown _projectMarkdown = new () {ExtraMode = true};

    protected override async Task OnInitializedAsync()
    {
        LangTablePreCacher!.Extra = new[] { "./index" };
        AppState!.ShowFooter = true;
        
        _model = await ProjectInfoGetter!.GetCorrespondingToUri();
        ParentLayout.Model = _model;
        AppState.PageIcon = _model!.Value.Header.PageIcon ?? StaticData.DefaultPageIcon;

        LanguageTable!.LanguageChangedAsync += OnLanguageChanged;
        await LanguageTable.AwaitLanguageContentAsync(SetLangData);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EnlargeContainer is null)
            await Console.Error.WriteLineAsync("EnlargeContainer is null");
        if (EnlargeContainer is not null)
        {
            await EnlargeContainer.LoadModule();
            if (firstRender)
                await SetEnlargerPageContent();
        }
        
    }

    private async Task SetPageData()
    {
        _model = await ProjectInfoGetter!.GetCorrespondingToUri();
        ParentLayout.Model = _model;

        if (AppState != null)
            AppState.PageIcon = _model!.Value.Header.PageIcon ?? StaticData.DefaultPageIcon;

        await SetLangData(this);
        await EnlargeContainer!.LoadModule();
        await SetEnlargerPageContent();
    }

    private Task OnLanguageChanged(object sender, int newCultureIdx) => SetLangData(sender);

    private async Task SetLangData(object sender)
    {
        var currentData = await LanguageTable!.LoadAllCurrentPageData();
        if (currentData == null)
        {
            await Console.Error.WriteLineAsync("Couldn't get Page Data in specified language!");
            return;
        }

        SetPageContent(currentData);
        await LangTablePreCacher!.PreCache(AppState!.CurrentLanguage);
    }

    private void SetPageContent(LangPageData model)
    {
        SetHeaderData(model.HeaderData);
        SetLinksData(model.LinksData);
        SetMarkdownData(model.PageMarkdownText);
        StateHasChanged();
    }

    private void SetHeaderData(LangHeaderModel? headerData)
    {
        if (headerData == null)
            return;
        var header = HeaderDataMapper!.MapFrom(headerData.Value);
        header.ImagePath = _model!.Value.Header.HeaderImage;
        AppState!.HeaderData = header;
        AppState.PageTitleExtension = " " + headerData.Value.PageTitleExtension;
    }

    private void SetLinksData(LangLinksModel? linksData)
    {
        if (linksData == null)
            return;

        var l = linksData.Value.Links.Length;
        _links = new NavLinkData[l];
        for (var i = 0; i < l; i++)
            _links[i] = NavLinkDataMapper!.MapFrom(linksData.Value.Links[i]);
        
        AppState!.Links = _links;
        AppState.MinNonStackedSize = linksData.Value.MinWidth;
    }

    private void SetMarkdownData(string? markdownText)
    {
        _markdownText = markdownText;
    }

    private ValueTask SetEnlargerPageContent()
    {
        return EnlargeContainer!.OnPageContentSet(".page-island.md img[title=open");
    }

    public void Dispose()
    {
        LanguageTable!.ManifestLoadedAsync -= SetLangData;
        LanguageTable.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}