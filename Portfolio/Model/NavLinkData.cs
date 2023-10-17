using Portfolio.Enums;

namespace Portfolio.Model;

public record NavLinkData(string Href, string LinkText, [property: Obsolete] bool OpensNew = false);