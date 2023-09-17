using Portfolio.Attributes;

namespace Portfolio.Model.Text;

public struct LangHeaderModel
{
    [CsvPropertyName("title")]
    public string Title { get; init; }
    [CsvPropertyName("under-title")]
    public string UnderTitle { get; init; }
    [CsvPropertyName("description")]
    public string Description { get; init; }
    [CsvPropertyName("page-title-extension")]
    public string PageTitleExtension { get; init; }
}