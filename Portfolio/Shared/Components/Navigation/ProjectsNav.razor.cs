using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Portfolio.Enums;
using Portfolio.Extensions;
using Portfolio.Model.Project;
using Portfolio.Services;

namespace Portfolio.Shared.Components.Navigation;

public partial class ProjectsNav : ComponentBase, IDisposable
{
    private ProjectDataModel? _activeProject;

    [Parameter]
    public ProjectDataModel? ActiveProject
    {
        get => _activeProject;
        set
        {
            // Avoid setting it to the same value
            if ((_activeProject?.LocalHref ?? "a") == (value?.LocalHref ?? "b"))
                return;

            var old = _activeProject;
            _activeProject = value;
            _ = ActiveProjectDelta(old, _activeProject);
        }
    }
    
    [Inject]
    private HttpClient? HttpClient { get; init; }
    [Inject]
    private ProjectInfoGetter? ProjectInfoGetter { get; init; }
    [Inject]
    private LanguageTable? LangTable { get; init; }
    [Inject]
    private AppState? AppState { get; init; }
    
    private string[]? _titles;
    private ProjectTags? _randomTag;
    private IReadOnlyDictionary<string, ProjectDataModel>? _relevantProjects;

    protected override async Task OnInitializedAsync()
    {
        await LangTable!.AwaitLanguageContentAsync(OnManifestLoaded); 
        LangTable.LanguageChangedAsync += OnLanguageChanged;

        if (ActiveProject is null)
            return;
        
        await ProjectInfoGetter!.RetrieveData();       
        _relevantProjects = GetRelevantProjects();
        StateHasChanged();
    }

    private async Task ActiveProjectDelta(ProjectDataModel? previous, ProjectDataModel? current)
    {
        _relevantProjects = GetRelevantProjects();
        _titles = await GetProjectTitles();
        StateHasChanged();
    }
    
    private Dictionary<string, ProjectDataModel>? GetRelevantProjects()
    {
        Dictionary<string, ProjectDataModel>? relevantProjects = new();
        var tryCount = 0;
        while (relevantProjects is null || relevantProjects.Count < 1)
        {
            if (tryCount > 4)
                break;
            
            var extractedTags = _activeProject?.Tags.ExtractFlags().ToArray();
            var random = extractedTags is not null ? Random.Shared.Next(extractedTags.Length) : 0;
            _randomTag = extractedTags?[random];

            if (_randomTag is null)
                break;

            relevantProjects = ProjectInfoGetter?.Data
                .Where(p => p.Value.Tags.HasFlag(_randomTag.Value) &&
                            p.Value.LocalHref != ActiveProject!.Value.LocalHref)
                .ToDictionary(key => key.Key, value => value.Value);
            
            tryCount++;
        }

        return relevantProjects;
    }

    private async Task<string[]?> GetProjectTitles()
    {
        var uris = _relevantProjects?.Keys.ToArray() ?? Array.Empty<string>();
        var length = uris.Length;
        var titles = new string[uris.Length];

        for (var i = 0; i < length; i++)
        {
            var headerData = await LangTable!.LoadHeaderForPage(uris[i], AppState!.CurrentLanguage);
            if (headerData is null)
                return default;
            titles[i] = headerData.Value.Title;
        }

        return titles;
    }

    private async Task OnManifestLoaded(object? sender)
    {
        _titles = await GetProjectTitles();
        StateHasChanged();
    }

    private async Task OnLanguageChanged(object? sender, int newLangIdx)
    {
        _titles = await GetProjectTitles();
        StateHasChanged();
    }
    
    public void Dispose()
    {
        LangTable!.ManifestLoadedAsync -= OnManifestLoaded;
        LangTable.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}