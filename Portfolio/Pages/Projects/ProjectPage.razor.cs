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
            if (!_hadFirstProjectRender)
                return;
            
            Task.Run(async () => { await SetPageData(); });
        }
    }


    [CascadingParameter]
    private ProjectLayout ParentLayout { get; init; } = null!;

    [Inject]
    private ContentProvider Provider { get; init; } = null!;
    
    [Inject]
    private AppState? AppState { get; init; }
    [Inject]
    private LanguageTable? LanguageTable { get; init; }
    [Inject]
    private IMapper<NewProjectMetaDataModel, HeaderData>? HeaderDataMapper { get; init; }
    [Inject]
    private IMapper<LangLinkModel, NavLinkData>? NavLinkDataMapper { get; init; }

    private ImageEnlargeContainer? EnlargeContainer { get; set; }
    
    private NavLinkData[]? _links;
    private NewProjectModel? _model;
    private string? _markdownText;
    private bool _hadFirstProjectRender;
    private readonly CancellationTokenSource _enlargerWaitToken = new();
    
    private readonly ProjectMarkdown _projectMarkdown = new () {ExtraMode = true};

    protected override async Task OnInitializedAsync()
    {
        await Provider.AwaitSupportedLanguages(SetPageData);

        AppState!.ShowFooter = true;
        LanguageTable!.LanguageChangedAsync += OnLanguageChanged;
    }

    // This function gets called on internally switching to the next project page
    // Note: doesn't get called when coming from a different page
    private async Task SetPageData()
    {
        _model = await Provider.GetProjectData(ProjectName);
        ParentLayout.Model = _model;
        
        if (AppState != null)
            AppState.PageIcon = _model.Value.Header.PageIcon ?? StaticData.DefaultPageIcon;

        await SetLangData(this);
    }

    private Task OnLanguageChanged(object sender, int newCultureIdx) => SetLangData(sender);
    
    private async Task SetLangData(object sender)
    {
        var metaTask = Provider.GetProjectMeta(_model!.Value.InformalName);
        var contentTask = Provider.GetProjectPageContent(_model!.Value.InformalName);

        await Task.WhenAll(metaTask, contentTask);
        
        SetPageContent(_model!.Value, metaTask.Result, contentTask.Result);
    }

    private void SetPageContent(NewProjectModel model, NewProjectMetaDataModel metaData, string pageContent)
    {
        _hadFirstProjectRender = true;
        
        SetHeaderData(metaData);
        SetMarkdownData(pageContent);
        StateHasChanged();
        
        _ = WaitForValidate(50, _enlargerWaitToken.Token);
    }

    private void SetHeaderData(NewProjectMetaDataModel? headerData)
    {
        if (headerData == null)
            return;
        var header = HeaderDataMapper!.MapFrom(headerData.Value);
        header.ImagePath = _model!.Value.Header.HeaderImage;
        AppState!.HeaderData = header;
        AppState.PageTitleExtension = " " + header.Title;
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

    private async ValueTask SetEnlargerPageContent()
    {
        if (EnlargeContainer is null)
            return;
        
        await EnlargeContainer.OnPageContentSet(".page-island.md img[title=open");
    }

    private async Task WaitForValidate(int waitDelay, CancellationToken cancellationToken)
    {
        while (!(EnlargeContainer?.IsModuleLoaded ?? false))
        {
            await Task.Delay(waitDelay, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
        }
        
        if (cancellationToken.IsCancellationRequested)
            return;

        await SetEnlargerPageContent();
    }

    public void Dispose()
    {
        _enlargerWaitToken.Cancel();
        LanguageTable!.ManifestLoadedAsync -= SetLangData;
        LanguageTable.LanguageChangedAsync -= OnLanguageChanged;
        Provider.OnSupportedLanguagesLoadedAsync -= SetPageData;
        GC.SuppressFinalize(this);
    }
}