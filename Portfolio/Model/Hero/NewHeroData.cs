using System.Text.Json.Serialization;

namespace Portfolio.Model.Hero;

public struct NewHeroData
{
    public string ParentPath { get; init; }
    [JsonPropertyName("value")]
    public NewHeroMeta LocalInfo { get; init; }
    public string[] RelatedProjects { get; init; }
}