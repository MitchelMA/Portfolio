using Microsoft.AspNetCore.Components;

namespace Portfolio.Services;

public static class LinkHelper
{
    public static string AsLocalHref(string href, NavigationManager manager)
    {
        string ret = href;
        if (href.StartsWith("#"))
            ret = manager.ToAbsoluteUri(manager.Uri).GetLeftPart(UriPartial.Path) + ret;

        return ret;
    }
}