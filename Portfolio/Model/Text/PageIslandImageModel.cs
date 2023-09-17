using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct PageIslandImageModel
{
    [JsonPropertyName("src")]
    public string Src { get; init; }
    [JsonPropertyName("alt")]
    public string Atl { get; init; }
    [JsonPropertyName("width")]
    public int? Width { get; init; }
    [JsonPropertyName("height")]
    public int? Height { get; init; }
    [JsonPropertyName("extra-css-styles")]
    public string? ExtraCssStyles { get; init; }
    
}