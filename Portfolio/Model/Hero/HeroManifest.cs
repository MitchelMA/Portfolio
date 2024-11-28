using System.Text.Json.Serialization;

namespace Portfolio.Model.Hero;

public struct HeroManifest
{
    [JsonPropertyName("HeroPrefix")]
    public string HeroPrefix { get; init; }
    [JsonPropertyName("RegisteredHeroes")]
    public string[] RegisteredHeroes { get; init; }
}