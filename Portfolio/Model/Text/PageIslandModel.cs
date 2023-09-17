using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct PageIslandModel
{
    [JsonPropertyName("Inverted")]
    public bool Inverted { get; init; }
    [JsonPropertyName("Flex-Values")]
    public string[] FlexValues { get; init; }
    [JsonPropertyName("Title")]
    public string Title { get; init; }
    public string HtmlContentString { get; set; }
    [JsonPropertyName("ImagesSticky")]
    public bool StickyImages { get; init; }
    [JsonPropertyName("Id")]
    public string Id { get; init; }
    [JsonPropertyName("Images")]
    public PageIslandImageModel[]? Images { get; init; }
}