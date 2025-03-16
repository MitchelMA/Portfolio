using Portfolio.Enums;

namespace Portfolio.Model.Project;

public struct NewProjectModel
{
    public string ParentPath { get; init; }
    public NewValueModel Value { get; init; }
    public Dictionary<string, string> TextContent { get; init; }
    public string[] RelatedSearches { get; init; }
}

public struct NewValueModel
{
    public string InformalName { get; init; }
    public string? GitHub { get; init; }
    public ProjectTags Tags { get; init; }
    public string[] Heroes { get; init; }
    public PlanningModel Planning { get; init; }
    public HeaderModel Header { get; init; }
}