using Portfolio.Enums;

namespace Portfolio.Model.Project;

public struct ProjectDataModel
{
    public Planning Planning { get; init; }
    public HeaderModel Header { get; init; }
    public ProjectStatus Status { get; init; }
    public string? TitleExtension { get; init; }
    public string LocalHref { get; init; }
    public string? GitHub { get; init; }
    public CardInfo CardInfo { get; init; }
}