using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Portfolio.Model.Project;

namespace Portfolio.Services;

public class ProjectInfoGetter
{
    private readonly HttpClient _client;
    private readonly NavigationManager _navigationManager;
    private readonly Dictionary<string, ProjectDataModel> _data = new();
    private readonly object _objLock = new();
    private ProjectDataModel[]? _pdm;

    public IReadOnlyDictionary<string, ProjectDataModel> Data => _data;

    public ProjectInfoGetter(HttpClient client, NavigationManager navigationManager)
    {
        _client = client;
        _navigationManager = navigationManager;
    }

    public async Task RetrieveData()
    {
        if (_data.Count > 0)
            return;

        _pdm ??= await _client.GetFromJsonAsync<ProjectDataModel[]>("./ProjectData/Projects.json");

        lock (_objLock)
        {
            // extra escape for when the lock unlocks for the next;
            if (_data.Count > 0)
                return;

            int l = _pdm!.Length;
            for (int i = 0; i < l; i++)
            {
                var model = _pdm[i];
                _ = AddOrReplace(model);
            }
        }
    }

    public async Task<ProjectDataModel?> GetCorrespondingToUri()
    {
        await RetrieveData();
        string path = "./" + _navigationManager.ToBaseRelativePath(_navigationManager.Uri).Split('#')[0].Split('?')[0];
        if (_data.TryGetValue(path, out var value))
            return value;

        return null;
    }

    public async Task<ProjectDataModel?> GetWithHref(string href)
    {
        await RetrieveData();
        if (_data.TryGetValue(href, out var value))
            return value;

        return null;
    }

    private bool AddOrReplace(ProjectDataModel model)
    {
        var added = _data.TryAdd(model.LocalHref, model);
        if (!added)
            _data[model.LocalHref] = model;

        return added;
    }
}