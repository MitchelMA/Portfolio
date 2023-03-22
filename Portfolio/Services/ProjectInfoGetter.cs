using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Portfolio.Model.Project;

namespace Portfolio.Services;

public class ProjectInfoGetter
{
    private HttpClient _client;
    private NavigationManager _navigationManager;
    private Dictionary<string, ProjectDataModel> _data = new();

    public ProjectInfoGetter(HttpClient client, NavigationManager navigationManager)
    {
        _client = client;
        _navigationManager = navigationManager;
    }

    private async Task RetrieveData()
    {
        if (_data.Count > 0)
            return;
        
        var data = 
            await _client.GetFromJsonAsync<ProjectDataModel[]>("./ProjectData/Projects.json");
        foreach (var model in data!)
        {
            _data.Add(model.LocalHref, model);
        }
        
    }

    public async Task<ProjectDataModel?> GetWithHref()
    {
        await RetrieveData();
        string path = "./" + _navigationManager.ToBaseRelativePath(_navigationManager.Uri).Split('#')[0];
        return _data[path];
    }
}