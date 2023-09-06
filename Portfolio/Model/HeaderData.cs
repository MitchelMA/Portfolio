namespace Portfolio.Model;

public class HeaderData : IComparable<HeaderData>
{
    public string? ImagePath;
    public string? Title;
    public string? UnderTitle;
    public string? Description;
    public string? ExtraStyles;

    public int CompareTo(HeaderData? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var imagePathComparison = string.Compare(ImagePath, other.ImagePath, StringComparison.Ordinal);
        if (imagePathComparison != 0) return imagePathComparison;
        var titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if (titleComparison != 0) return titleComparison;
        var underTitleComparison = string.Compare(UnderTitle, other.UnderTitle, StringComparison.Ordinal);
        if (underTitleComparison != 0) return underTitleComparison;
        var descriptionComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
        if (descriptionComparison != 0) return descriptionComparison;
        return string.Compare(ExtraStyles, other.ExtraStyles, StringComparison.Ordinal);
    }
}