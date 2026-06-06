using Microsoft.JSInterop;

namespace Portfolio.Services;

public class JavascriptModuleHandle : IAsyncDisposable
{
    private readonly string _moduleName;
    private readonly SemaphoreSlim _moduleSema = new(1);
    private readonly IJSRuntime? _jsRuntime;
    private IJSObjectReference? _module;

    public JavascriptModuleHandle(IJSRuntime jsRuntime, string moduleName)
    {
        _jsRuntime = jsRuntime;
        _moduleName = moduleName;
    }

    public bool IsModuleLoaded => _module is not null;

    public async ValueTask<bool> ImportJsModuleAsync()
    {
        await _moduleSema.WaitAsync();
        
        try
        {
            if (_jsRuntime is null)
            {
                await Console.Error.WriteLineAsync("JsRuntime was null!");
                return false;
            }

            if (IsModuleLoaded)
            {
                Console.WriteLine($"Module \'{_moduleName}\' already loaded");
                return true;
            }

            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", _moduleName);
            return true;
        }
        finally
        {
            _moduleSema.Release();
        }
    }

    public async ValueTask<bool> CloseModuleAsync()
    {
        await _moduleSema.WaitAsync();
        
        try
        {
            if (!IsModuleLoaded)
                return false;
            
            await _module!.DisposeAsync();
            _module = null;
            return true;
        }
        finally
        {
            _moduleSema.Release();
        }
    }

    public async ValueTask<TReturn?> InvokeAsync<TReturn>(string identifiers, params object?[] args)
    {
        await _moduleSema.WaitAsync();

        try
        {
            if (!IsModuleLoaded)
            {
                await Console.Error.WriteLineAsync("Module was null!");
                return default;
            }

            return await _module!.InvokeAsync<TReturn>(identifiers, args);
        }
        finally
        {
            _moduleSema.Release();
        }
    }

    public async ValueTask InvokeVoidAsync(string identifiers, params object?[] args)
    {
        if (!IsModuleLoaded)
        {
            await Console.Error.WriteLineAsync("Module was null!");
            return;
        }

        await _moduleSema.WaitAsync();

        try
        {
            await _module!.InvokeVoidAsync(identifiers, args);
        }
        finally
        {
            _moduleSema.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await CloseModuleAsync();
        GC.SuppressFinalize(this);
    }
}