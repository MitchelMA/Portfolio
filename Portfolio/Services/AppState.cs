namespace Portfolio.Services;

public class AppState
{
    private bool _showFooter = true;
    private string _headerImgPath = "icon-192.png";

    public event Action StateChanged;
    private void NotifyStateChanged() => StateChanged?.Invoke();

    public bool ShowFooter
    {
        get => _showFooter;
        set
        {
            if (value == _showFooter)
                return;

            _showFooter = value;
            NotifyStateChanged();
        }
    }
    
    public string HeaderImgPath
    {
        get => _headerImgPath;
        set
        {
            if (value == _headerImgPath)
                return;

            _headerImgPath = value;
            NotifyStateChanged();
        }
    }
}