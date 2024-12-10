using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Portfolio.Services;

public class EnlargeImageService : IDisposable, IAsyncDisposable
{
    public EnlargeImageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public delegate
        bool OnImageClickedDelegate(object? sender, string imageSrc, string imageAlt, Vector2 origin, Vector2 size);
    public delegate
        Task<bool> OnImageClickedDelegateAsync(object? sender, string imageSrc, string imageAlt, Vector2 origin, Vector2 size);

    public static OnImageClickedDelegate? OnImageClicked;
    public static OnImageClickedDelegateAsync? OnImageClickedAsync;

    private readonly IJSRuntime? _jsRuntime;
    private IJSObjectReference? _module;
    private string? _lastUsedQuery;

    public bool IsModuleLoaded => _module is not null;

    public async ValueTask ImportJsModule(string moduleName)
    {
        if (_jsRuntime is null)
        {
            await Console.Error.WriteLineAsync("JsRuntime was null!");
            return;
        }

        _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import",
            moduleName);
    }

    public async ValueTask AddImageHandlers(string javaScriptQuery)
    {
        if (_module is null)
        {
            await Console.Error.WriteLineAsync("Module was null!");
            return;
        }

        _lastUsedQuery = javaScriptQuery;
        await _module.InvokeVoidAsync("addImageHandlers", javaScriptQuery);
    }

    public ValueTask<float[]> GetScreenSize()
    {
        return _module!.InvokeAsync<float[]>("getScreenSize");
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
            await _module.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    #endregion
}