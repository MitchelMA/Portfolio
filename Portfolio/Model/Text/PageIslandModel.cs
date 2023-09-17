using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct PageIslandModel
{
    [JsonPropertyName("inverted")]
    public bool Inverted { get; init; }
    [JsonPropertyName("flex-values")]
    public string[] FlexValues { get; init; }
    [JsonPropertyName("title")]
    public string Title { get; init; }
    public string HtmlContentString { get; set; }
    [JsonPropertyName("sticky-images")]
    public bool StickyImages { get; init; }
    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("images")]
    public PageIslandImageModel[]? Images { get; init; }
}