using Portfolio.Enums;

namespace Portfolio.Model.Project;

public struct PlanningModel
{
    public int Duration { get; init; }
    public Durations DurationType { get; init; }
    public int PeopleCount { get; init; }
    public string Software { get; init; }
}