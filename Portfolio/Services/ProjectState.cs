using Portfolio.Enums;
using Portfolio.Model;

namespace Portfolio.Services;

public class ProjectState
{
    private ProjectPageEnd _projectPageEnd = new();

    public event Action? StateChanged;
    public event Func<Task>? StateChangedAsync;

    private void NotifyStateChanged()
    {
        StateChangedAsync?.Invoke();
        StateChanged?.Invoke();
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