using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Portfolio.Client;
using Portfolio.Model.Project;
using Portfolio.Services;

namespace Portfolio.Shared.Layouts;

public partial class ProjectLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private AppState? AppState { get; init; }
    [Inject]
    private IJSRuntime? JsRuntime { get; init; }
    
    private NewProjectModel? _model;
    public NewProjectModel? Model
    {
        get => _model;
        set
        {
            _model = value;   
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        AppState!.StateChangedAsync += AppStateChanged;
    }

    private async Task AppStateChanged()
    {
        await CheckScroll();
        StateHasChanged();
    }

    private ValueTask CheckScroll()
    {
        return JsRuntime!.InvokeVoidAsync(AppState!.ScrollLocked ? "lockScroll" : "unlockScroll", StaticData.LockedClassName);
    }
    
    public void Dispose()
    {
        AppState!.StateChangedAsync -= AppStateChanged;
        GC.SuppressFinalize(this);
    }
}
