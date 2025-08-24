using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Portfolio.Client;
using Portfolio.Model.Project;
using Portfolio.Services;
using Portfolio.Shared.Components.Lightbox;

namespace Portfolio.Shared.Layouts;

public partial class ProjectLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private AppState? AppState { get; init; }
    [Inject]
    private IJSRuntime? JsRuntime { get; init; }
    
    private ProjectDataModel? _model;
    public ProjectDataModel? Model
    {
        get => _model;
        set
        {
            _model = value;   
            StateHasChanged();
        }
    }
    
    private ImageEnlargeContainer? EnlargeContainer { get; set; }

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

    public async Task OnChildContentSet()
    {
        // Force synchronous running of async code
        await Task.Run(async () =>
        {
            await EnlargeContainer!.LoadModule();
            await SetEnlargerPageContent();
        });
    }
    
    private async ValueTask SetEnlargerPageContent()
    {
        if (EnlargeContainer is null)
            return;
        
        await EnlargeContainer.OnPageContentSet(".page-island.md img[title=open]");
    }
    
    public void Dispose()
    {
        AppState!.StateChangedAsync -= AppStateChanged;
        GC.SuppressFinalize(this);
    }
}
