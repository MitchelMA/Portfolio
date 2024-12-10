using Portfolio.Enums;
using Portfolio.Model.Text;

namespace Portfolio.Model;

public struct CarouselModel
{
    public string Background { get; init; }
    public string? Href { get; init; }
    public string? InformalName { get; init; }
    public string? SetHeight { get; init; }
    
    public ProjectTags Tags { get; init; }

    public LangHeaderModel HeaderData { get; set; }
}