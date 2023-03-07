using Portfolio.Client;
using Portfolio.Model.Tags;

namespace Portfolio.Model;

public class PageDetails
{
    public string TitleExtension = string.Empty;
    public LinkTag Icon = StaticData.DefaultPageIcon;
    public bool ShowFooter = true;
    public (string, string)[]? Links;
}