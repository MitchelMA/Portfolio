using Portfolio.Enums;

namespace Portfolio.Services;

public class ProjectState
{
    private ProjectStatus _projectStatus;

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
}