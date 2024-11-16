using Portfolio.Attributes;

namespace Portfolio.Model.Hero;

public struct HeroPageInfo
{
    [CsvPropertyName("title")]
    public string Title { get; init; }
    
    [CsvPropertyName("description")]
    public string UnderTitle { get; init; }
    
    [CsvPropertyName("page-title-extension")]
    public string PageTitleExtension { get; init; }
    
    [CsvPropertyName("page-icon")]
    public string PageIcon { get; init; }
}