using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Portfolio.Services;

namespace Portfolio.Shared.Components.Lightbox;

public partial class ImageEnlargeContainer : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject] private EnlargeImageService EnlargeImageService { get; init; } = null!;

    [Inject] private AppState AppState { get; init; } = null!;

    private Vector2 _currentPosition;

    private string? _clickedSrc;
    private string? _clickedAlt;
    private bool _isTransitioning;
    private bool _isOpen = false;

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


    public async ValueTask LoadModule()
    {
        if (!EnlargeImageService.IsModuleLoaded)
            await EnlargeImageService.ImportJsModule("./js/modules/EnlargeImageModule.js");

        EnlargeImageService.OnImageClickedAsync += OnImageClickedAsync;
    }

    public ValueTask OnPageContentSet(string query)
    {
        return EnlargeImageService.AddImageHandlers(query);
    }

    private async Task<bool> OnImageClickedAsync(object? sender, string imageSrc, string imageAlt, Vector2 origin,
        Vector2 size)
    {
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
        return false;
    }
    
    private void Close()
    {
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
        EnlargeImageService.OnImageClickedAsync -= OnImageClickedAsync;
        AppState.ForceScrollUnlock();
        
        await EnlargeImageService.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}