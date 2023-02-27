namespace Portfolio.Model.Tags;

public class PageIcon : LinkTag
{
    public PageIcon(string mimeType, string src)
    {
        Attributes = new Dictionary<string, object>
        {
            { "rel", "icon" },
            { "type", mimeType },
            { "href", src }
        };
    }
}