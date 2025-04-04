using System.Drawing;
using System.Text.Json.Serialization;
using Portfolio.Attributes;
using Portfolio.Converters.JSON;

namespace Portfolio.Model.Hero;

public struct HeroPageInfo
{
    [CsvPropertyName("title")]
    [JsonPropertyName("title")]
    public string Title { get; init; }
    [CsvPropertyName("description")]
    [JsonPropertyName("description")]
    public string UnderTitle { get; init; }
    [CsvPropertyName("page-title-extension")]
    [JsonPropertyName("page-title-extension")]
    public string PageTitleExtension { get; init; }
    [CsvPropertyName("page-icon")]
    [JsonPropertyName("page-icon")]
    public string PageIcon { get; init; }
    [CsvPropertyName("banner-img")]
    [JsonPropertyName("banner-img")]
    public string BannerImg { get; init; }
    [CsvPropertyName("theme-colour")]
    [JsonPropertyName("theme-colour")]
    [JsonConverter(typeof(JsonColourConverter))]
    public Color ThemeColour { get; init; }
}