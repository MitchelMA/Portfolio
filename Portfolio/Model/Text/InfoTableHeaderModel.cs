using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct InfoTableHeaderModel
{
    [JsonPropertyName("duration-text")]
    public string DurationText { get; init; }
    [JsonPropertyName("group-size-text")]
    public string GroupSizeText { get; init; }
    [JsonPropertyName("software-text")]
    public string SoftwareText { get; init; }
}