using System.Text.Json.Serialization;
using Portfolio.Attributes;

namespace Portfolio.Model.Text;

public struct LangLinksModel
{
    [JsonPropertyName("min-width")]
    public int MinWidth { get; init; }
    [JsonPropertyName("links")]
    public LangLinkModel[] Links { get; init; }
}