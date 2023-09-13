using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct DurationTextsModel
{
    [JsonPropertyName("singular")]
    public string[] Singular { get; init; }
    [JsonPropertyName("plural")]
    public string[] Plural { get; init; }
}