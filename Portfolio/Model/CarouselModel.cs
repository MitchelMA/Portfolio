namespace Portfolio.Model;

public struct CarouselModel
{
    public string Header { get; init; }
    public string Text { get; init; }
    public string Background { get; set; }
    public string? Href { get; init; }
}