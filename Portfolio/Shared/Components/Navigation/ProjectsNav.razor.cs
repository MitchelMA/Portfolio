using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Portfolio.Enums;
using Portfolio.Extensions;
using Portfolio.Model.Project;
using Portfolio.Model.Text;
using Portfolio.Services;

namespace Portfolio.Shared.Components.Navigation;

public partial class ProjectsNav : ComponentBase, IDisposable
{
    private NewProjectModel? _activeProject;

    [Parameter]
    public NewProjectModel? ActiveProject
    {
        get => _activeProject;
        set
        {
            // Avoid setting it to the same value
            if ((_activeProject?.InformalName ?? "a") == (value?.InformalName ?? "b"))
                return;

            var old = _activeProject;
            _activeProject = value;
            _ = ActiveProjectDelta(old, _activeProject);
        }
    }
    
    [Inject]
    private HttpClient? HttpClient { get; init; }
    [Inject]
    private ContentProvider Provider { get; init; } = null!;
    [Inject]
    private LanguageTable? LangTable { get; init; }
    [Inject]
    private AppState? AppState { get; init; }
    
    private string[]? _titles;
    private ProjectTags? _randomTag;
    private IReadOnlyDictionary<string, NewProjectModel>? _relevantProjects;

    protected override async Task OnInitializedAsync()
    {
        LangTable!.LanguageChangedAsync += OnLanguageChanged;

        _relevantProjects = await GetRelevantProjects();
        await Provider.AwaitSupportedLanguages(OnManifestLoaded);
    }

    private async Task ActiveProjectDelta(NewProjectModel? previous, NewProjectModel? current)
    {
        _relevantProjects = await GetRelevantProjects();
        _titles = await GetProjectTitles();
        StateHasChanged();
    }
    
    private async Task<Dictionary<string, NewProjectModel>?> GetRelevantProjects()
    {
        var extractedTags = _activeProject?.Tags.ExtractFlags().ToArray();
        var random = extractedTags is not null ? Random.Shared.Next(extractedTags.Length) : 0;
        _randomTag = extractedTags?[random];

        if (_randomTag is null)
            return null;

        Dictionary<string, NewProjectModel> relevantProjects = (await Provider.GetProjectsWithTags(_randomTag!.Value))
            .ToDictionary(x => x.InformalName);

        return relevantProjects;
    }

    private async Task<string[]?> GetProjectTitles()
    {
        var informals = _relevantProjects?.Keys.ToArray() ?? Array.Empty<string>();
        var length = informals.Length;
        Task<NewProjectMetaDataModel>[] titleRequests = new Task<NewProjectMetaDataModel>[informals.Length];

        for (int i = 0; i < length; i++)
            titleRequests[i] = Provider.GetProjectMeta(informals[i]);

        await Task.WhenAll(titleRequests);

        return titleRequests.Select(x => x.Result.Title).ToArray();
    }

    private async Task OnManifestLoaded()
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
        Provider.OnSupportedLanguagesLoadedAsync -= OnManifestLoaded;
        LangTable!.LanguageChangedAsync -= OnLanguageChanged;
        GC.SuppressFinalize(this);
    }
}