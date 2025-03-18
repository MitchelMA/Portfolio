using System.Drawing;
using System.Text.Json.Serialization;
using Portfolio.Converters.JSON;

namespace Portfolio.Model.Hero;

public struct NewHeroMeta
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string? PageIcon { get; init; }
    public string BannerImage { get; init; }
    
    [JsonConverter(typeof(JsonColorConverter))]
    public Color? ThemeColour { get; init; }
}