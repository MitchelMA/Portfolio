namespace Portfolio.Model;

public class HeaderData : IComparable<HeaderData>
{
    public string? ImagePath;
    public string? Title;
    public string? UnderTitle;

    public int CompareTo(HeaderData? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        var imagePathComparison = string.Compare(ImagePath, other.ImagePath, StringComparison.Ordinal);
        if (imagePathComparison != 0) return imagePathComparison;
        
        var headerTitleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if (headerTitleComparison != 0) return headerTitleComparison;
        
        return string.Compare(UnderTitle, other.UnderTitle, StringComparison.Ordinal);
    }
}