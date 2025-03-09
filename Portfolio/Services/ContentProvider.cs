using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Model.Project;

namespace Portfolio.Services;

public sealed class ContentProvider
{
    private readonly string _contentLocation;
    private readonly string _projectsSubPath;
    private readonly string _heroesSubPath;

    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _httpClient;

    public ContentProvider(IConfiguration configuration, IMemoryCache memoryCache)
    {
        _contentLocation = configuration["ContentLocation:Base"];
        _projectsSubPath = configuration["ContentLocation:Projects"];
        _heroesSubPath   = configuration["ContentLocation:Heroes"];

        _memoryCache = memoryCache;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetProjectData(string informalName)
    {
        var message = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(_contentLocation + _projectsSubPath + informalName)
        );
        message.SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);
        var response = await _httpClient.SendAsync(message);

        return await response.Content.ReadAsStringAsync();
    }
}