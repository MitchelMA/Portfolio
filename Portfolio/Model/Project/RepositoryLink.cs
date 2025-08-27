using Portfolio.Enums;

namespace Portfolio.Model.Project;

public readonly struct ProjectRepositoryLink
{
    public VersionControlOriginType OriginType { get; init; }
    public string UserName { get; init; }
    public string RepositoryName { get; init; }
    public bool? UsesPages { get; init; }

    public string RepositoryLink()
    {
        return "https://" +
               OriginType.ToString().ToLower() + ".com/" +
               UserName + '/' +
               RepositoryName;
    }

    public string PagesLink()
    {
        return "https://" +
               UserName.ToLower() + '.' +
               OriginType.ToString().ToLower() + ".io/" +
               RepositoryName;
    }
}