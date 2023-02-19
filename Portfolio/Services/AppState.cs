namespace Portfolio.Services;

public class AppState
{
    private bool _showFooter = true;
    private string _headerImgPath = "icon-192.png";
    private (string, string)[] _links;

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

    public (string, string)[] Links
    {
        get => _links;
        set
        {
            _links = value;
            NotifyStateChanged();
        }
    }
}