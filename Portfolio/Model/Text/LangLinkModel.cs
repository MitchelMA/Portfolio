using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct LangLinkModel
{
    [JsonPropertyName("href")]
    public string Href { get; init; }
    [JsonPropertyName("display-text")]
    public string DisplayText { get; init; }
    [JsonPropertyName("opens-new")]
    public bool OpensNew { get; init; }
}