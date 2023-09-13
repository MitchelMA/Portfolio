using Portfolio.Attributes;

namespace Portfolio.Model.Text;

public struct InfoTableHeaderModel
{
    [CsvPropertyName("duration-text")]
    public string DurationText { get; init; }
    [CsvPropertyName("group-size-text")]
    public string GroupSizeText { get; init; }
    [CsvPropertyName("software-text")]
    public string SoftwareText { get; init; }
}