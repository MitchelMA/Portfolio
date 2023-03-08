namespace Portfolio.Services;

public class ProjectState
{
    public event Action? StateChanged;
    public event Func<Task>? StateChangedAsync;

    private void NotifyStateChanged()
    {
        StateChangedAsync?.Invoke();
        StateChanged?.Invoke();
    }
}