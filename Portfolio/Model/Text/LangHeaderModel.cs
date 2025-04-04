using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct LangHeaderModel
{
    [JsonPropertyName("title")]
    public string Title { get; init; }
    [JsonPropertyName("under-title")]
    public string UnderTitle { get; init; }
    [JsonPropertyName("description")]
    public string Description { get; init; }
    [JsonPropertyName("page-title-extension")]
    public string PageTitleExtension { get; init; }
}