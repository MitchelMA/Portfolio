using System.Numerics;
using Microsoft.JSInterop;

namespace Portfolio.Services;

public class EnlargeImageService : IAsyncDisposable
{
    public EnlargeImageService(IJSRuntime jsRuntime)
    {
        _moduleHandle = new JavascriptModuleHandle(jsRuntime, "/js/modules/EnlargeImageModule.js");
    }

    public delegate
        bool OnImageClickedDelegate(object? sender, string imageSrc, string imageAlt, Vector2 origin, Vector2 size);
    public delegate
        Task<bool> OnImageClickedDelegateAsync(object? sender, string imageSrc, string imageAlt, Vector2 origin, Vector2 size);

    public static OnImageClickedDelegate? OnImageClicked;
    public static OnImageClickedDelegateAsync? OnImageClickedAsync;

    private readonly JavascriptModuleHandle _moduleHandle;

    public bool IsModuleLoaded => _moduleHandle.IsModuleLoaded;

    public ValueTask<bool> ImportJsModule()
    {
        return _moduleHandle.ImportJsModuleAsync();
    }

    public ValueTask<int> AddImageHandlers(string javaScriptQuery)
    {
        return _moduleHandle.InvokeAsync<int>("addImageHandlers", javaScriptQuery);
    }

    public async ValueTask<float[]> GetScreenSize()
    {
        return (await _moduleHandle.InvokeAsync<float[]>("getScreenSize")) ?? Array.Empty<float>();
    }

    [JSInvokable]
    public static async Task<bool>? EnlargeImage(string imageSrc, string imageAlt, float originX, float originY, float width, float height)
    {
        var value = false;
        value |= OnImageClicked?.Invoke(null, imageSrc, imageAlt, 
            new Vector2(originX, originY), new Vector2(width, height)) ?? false;
        value |= OnImageClickedAsync is not null && await OnImageClickedAsync.Invoke(null, imageSrc, imageAlt,
            new Vector2(originX, originY), new Vector2(width, height));
        return value;
    }

    #region Disposables

    public async ValueTask DisposeAsync()
    {
        await _moduleHandle.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    #endregion
}