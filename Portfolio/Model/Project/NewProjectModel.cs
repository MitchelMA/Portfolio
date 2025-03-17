using Portfolio.Enums;

namespace Portfolio.Model.Project;

public struct NewProjectModel
{
    public string InformalName { get; init; }
    public string? GitHub { get; init; }
    public ProjectTags Tags { get; init; }
    public string[] Heroes { get; init; }
    public PlanningModel Planning { get; init; }
    public HeaderModel Header { get; init; }
}