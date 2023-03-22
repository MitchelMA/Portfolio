using Portfolio.Model;
using Portfolio.Model.Tags;

namespace Portfolio.Services;

public class AppState
{
    private PageDetails _pageDetails = new();
    private HeaderData _headerData = new();
    private readonly List<object> _scrollLocks = new();

    public event Action? StateChanged;
    public event Func<Task>? StateChangedAsync;

    private void NotifyStateChanged()
    {
        StateChangedAsync?.Invoke();
        StateChanged?.Invoke();
    }
    
    public PageDetails PageDetails
    {
        get => _pageDetails;
        set
        {
            _pageDetails = value;
            NotifyStateChanged();
        }
    }

    public HeaderData HeaderData
    {
        get => _headerData;
        set
        {
            if (_headerData == value)
                return;

            _headerData = value;
            NotifyStateChanged();
        }
    }
    
    public string? PageTitleExtension
    {
        get => PageDetails.TitleExtension;
        set
        {
            if (value == PageDetails.TitleExtension)
                return;

            PageDetails.TitleExtension = value;
            NotifyStateChanged();
        }
    }

    public LinkTag PageIcon
    {
        get => PageDetails.Icon;
        set
        {
            PageDetails.Icon = value;
            NotifyStateChanged();
        }
    }

    public bool ShowFooter
    {
        get => PageDetails.ShowFooter;
        set
        {
            if (value == PageDetails.ShowFooter)
                return;

            PageDetails.ShowFooter = value;
            NotifyStateChanged();
        }
    }

    public NavLinkData[]? Links
    {
        get => PageDetails.Links;
        set
        {
            PageDetails.Links = value;
            NotifyStateChanged();
        }
    }

    public int MinNonStackedSize
    {
        get => PageDetails.MinNonStackedSize;
        set
        {
            if (value == PageDetails.MinNonStackedSize)
                return;

            PageDetails.MinNonStackedSize = value;
            NotifyStateChanged();
        }
    }
    
    public string? HeaderImgPath
    {
        get => HeaderData.ImagePath;
        set
        {
            if (value == HeaderData.ImagePath)
                return;

            HeaderData.ImagePath = value;
            NotifyStateChanged();
        }
    }

    public string? HeaderTitle
    {
        get => HeaderData.Title;
        set
        {
            if (value == HeaderData.Title)
                return;

            HeaderData.Title = value;
            NotifyStateChanged();
        }
    }

    public string? HeaderUnderTitle
    {
        get => HeaderData.UnderTitle;
        set
        {
            if (value == HeaderData.UnderTitle)
                return;

            HeaderData.UnderTitle = value;
            NotifyStateChanged();
        }
    }

    public string? HeaderExtraStyles
    {
        get => _headerData.ExtraStyles;
        set
        {
            if (value == _headerData.ExtraStyles)
                return;

            _headerData.ExtraStyles = value;
            NotifyStateChanged();
        }
    }

    public bool ScrollLocked => _scrollLocks.Count > 0;
    
    public bool IsLocking(object obj) => _scrollLocks.Contains(obj);

    public bool AddToLockScroll(object obj)
    {
        if (IsLocking(obj))
            return false;
        
        _scrollLocks.Add(obj);
        NotifyStateChanged();
        return true;
    }

    public bool RemoveFromScrollLock(object obj)
    {
        bool success = _scrollLocks.Remove(obj);
        if(success)
            NotifyStateChanged();

        return success;
    }

    public void ForceScrollUnlock()
    {
        _scrollLocks.Clear();
        NotifyStateChanged();
    }
}