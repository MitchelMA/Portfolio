using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Portfolio.Services;

namespace Portfolio.Shared.Components.Lightbox;

public partial class ImageEnlargeContainer : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject] private EnlargeImageService EnlargeImageService { get; init; } = null!;
    [Inject] private AppState AppState { get; init; } = null!;

    public bool IsModuleLoaded => EnlargeImageService.IsModuleLoaded;

    private Vector2 _currentPosition;

    private string? _clickedSrc;
    private string? _clickedAlt;
    private bool _isTransitioning;
    private bool _isOpen = false;
    private Mutex _loadMutex = new();
    private bool _isBound = false;

    public delegate void OnModuleLoadedDelegate(object? sender);

    public delegate ValueTask OnModuleLoadedAsyncDelegate(object? sender);

    public event OnModuleLoadedDelegate? OnModuleLoaded;
    public event OnModuleLoadedAsyncDelegate? OnModuleLoadedAsync;

    private string? TransitionStyle
    {
        get
        {
            if (!_isTransitioning)
                return null;
            return $"left:{Math.Floor(_currentPosition.X).ToString(CultureInfo.InvariantCulture)}px;" +
                   $"top: {Math.Floor(_currentPosition.Y).ToString(CultureInfo.InvariantCulture)}px;" +
                   "transition: all 500ms ease 0s;";

        }
    }

    private string? OpenClass => _isOpen ? "open" : null;

    protected override async Task OnInitializedAsync()
    {
        await LoadModule();
    }

    public async ValueTask LoadModule()
    {
        _loadMutex.WaitOne();
        if (EnlargeImageService.IsModuleLoaded || _isBound) return;
        _isBound = true;
        
        await EnlargeImageService.ImportJsModule("./js/modules/EnlargeImageModule.js");
        EnlargeImageService.OnImageClickedAsync += OnImageClickedAsync;
        OnModuleLoaded?.Invoke(this);
        if (OnModuleLoadedAsync is not null)
            await OnModuleLoadedAsync.Invoke(this);
        _loadMutex.ReleaseMutex();
    }

    public async ValueTask<int> OnPageContentSet(string query)
    {
        return await EnlargeImageService.AddImageHandlers(query);
    }

    private async Task<bool> OnImageClickedAsync(object? sender, string imageSrc, string imageAlt, Vector2 origin,
        Vector2 size)
    {
        if (_isTransitioning || _isOpen)
            return true;
        
        _isOpen = true;
        _clickedSrc = imageSrc;
        _clickedAlt = imageAlt;
        AppState.AddToLockScroll(this);
        
        var screenSize = await EnlargeImageService.GetScreenSize();
        var screenCentre = new Vector2(
            screenSize[0] / 2f,
            screenSize[1] / 2f
        );
        var offset = ((origin + size/2f) - screenCentre) / 3;
        _currentPosition = offset;
        _isTransitioning = true;
        StateHasChanged();
        
        await Task.Delay(25);
        _currentPosition = Vector2.Zero;
        StateHasChanged();
        
        await Task.Delay(500);
        _isTransitioning = false;
        StateHasChanged();
        return true;
    }
    
    private void Close()
    {
        if (_isTransitioning)
            return;
        
        _isOpen = false;
        AppState.RemoveFromScrollLock(this);
        StateHasChanged();
    }

    public void Dispose()
    {
        EnlargeImageService.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        Close();
        EnlargeImageService.OnImageClickedAsync -= OnImageClickedAsync;
        
        await EnlargeImageService.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}