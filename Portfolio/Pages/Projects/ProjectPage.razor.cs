using Microsoft.AspNetCore.Components;
using Portfolio.Client;
using Portfolio.Mappers;
using Portfolio.Model;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
using Portfolio.Services;
using Portfolio.Services.Markdown;
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
    private AppState? AppState { get; init; }
    [Inject]
    private ProjectInfoGetter? ProjectInfoGetter { get; init; }
    [Inject]
    private LanguageTable? LanguageTable { get; init; }
    
    [Inject]
    private IMapper<LangHeaderModel, HeaderData>? HeaderDataMapper { get; init; }
    [Inject]
    private IMapper<LangLinkModel, NavLinkData>? NavLinkDataMapper { get; init; }

    private NavLinkData[]? _links;
    private ProjectDataModel? _model;
    private string? _markdownText;
    private bool _hadFirstProjectRender;
    
    private readonly ProjectMarkdown _projectMarkdown = new () {ExtraMode = true};

    #region Inherited Overrides
    protected override async Task OnInitializedAsync()
    {
        LanguageTable!.LanguageChangedAsync += OnLanguageChanged;
        
        AppState!.ShowFooter = true;
        
        _model = await ProjectInfoGetter!.GetWithHref(ProjectName);
        ParentLayout.Model = _model;
        AppState.PageIcon = _model!.Value.Header.PageIcon ?? StaticData.DefaultPageIcon;
        
        await LanguageTable!.AwaitLanguageContentAsync(SetLangData);
    }

    #endregion
    
    private Task OnLanguageChanged(object sender, int newCultureIdx) => SetLangData(sender);

    #region Data Setters
    private async Task SetPageData()
    {
        _model = await ProjectInfoGetter!.GetWithHref(ProjectName);
        ParentLayout.Model = _model;

        if (AppState != null)
            AppState.PageIcon = _model!.Value.Header.PageIcon ?? StaticData.DefaultPageIcon;

        await SetLangData(this);
    }
    
    private async Task SetLangData(object sender)
    {
        var currentData = await LanguageTable!.GetAllPageData(ProjectName);
        if (currentData == null)
        {
            await Console.Error.WriteLineAsync("Couldn't get Page Data in specified language!");
            return;
        }

        await SetPageContent(currentData);
    }

    private async Task SetPageContent(LangPageData model)
    {
        _hadFirstProjectRender = true;
        SetHeaderData(model.HeaderData);
        SetLinksData(model.LinksData);
        SetMarkdownData(model.PageMarkdownText);
        StateHasChanged();
        
        // Notify the parent that the language-content of the child has been set
        await ParentLayout.OnChildContentSet();
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
    #endregion 
    
    #region Disposable
    public void Dispose()
    {
        LanguageTable!.ManifestLoadedAsync -= SetLangData;
        LanguageTable.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
    #endregion
}