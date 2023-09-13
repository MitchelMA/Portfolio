using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct PageIslandImageModel
{
    [JsonPropertyName("Src")]
    public string Src { get; init; }
    [JsonPropertyName("Alt")]
    public string Atl { get; init; }
    [JsonPropertyName("Width")]
    public int? Width { get; init; }
    [JsonPropertyName("Height")]
    public int? Height { get; init; }
    [JsonPropertyName("ExtraCSSStyles")]
    public string? ExtraCssStyles { get; init; }
    
}