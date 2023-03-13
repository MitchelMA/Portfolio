using Portfolio.Enums;
using Portfolio.Model;

namespace Portfolio.Services;

public class ProjectState
{
    private ProjectStatus _projectStatus;
    private ProjectPageEnd _projectPageEnd = new();

    public event Action? StateChanged;
    public event Func<Task>? StateChangedAsync;

    private void NotifyStateChanged()
    {
        StateChangedAsync?.Invoke();
        StateChanged?.Invoke();
    }

    public ProjectStatus ProjectStatus
    {
        get => _projectStatus;
        set
        {
            if (value == _projectStatus)
                return;

            _projectStatus = value;
            NotifyStateChanged();
        }
    }

    public ProjectPageEnd ProjectPageEnd
    {
        get => _projectPageEnd;
        set
        {
            if (value == _projectPageEnd)
                return;

            _projectPageEnd = value;
            NotifyStateChanged();
        }
    }

    public string? PageEndGithub
    {
        get => ProjectPageEnd.GitHubLink;
        set
        {
            if (value == ProjectPageEnd.GitHubLink)
                return;

            ProjectPageEnd.GitHubLink = value;
            NotifyStateChanged();
        }
    }
}