using Portfolio.Client;
using Portfolio.Model.Tags;

namespace Portfolio.Model;

public class PageDetails
{
    public LinkTag Icon = StaticData.DefaultPageIcon;
    public bool ShowFooter = true;
    public (string, string)[]? Links;
}