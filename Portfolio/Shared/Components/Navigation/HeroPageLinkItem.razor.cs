using System.Drawing;
using Microsoft.AspNetCore.Components;
using Portfolio.Extensions;
using Portfolio.Services;

namespace Portfolio.Shared.Components.Navigation;

public partial class HeroPageLinkItem : ComponentBase, IDisposable
{
    private Color? _heroColour;
    private string? _heroName;

    [Parameter]
    public string? HeroName
    {
        get => _heroName;
        set
        {
            if (value == _heroName)
                return;
            
            _heroName = value;
            _ = SetHeroColour();
        }
    }
    [Parameter]
    public string? SizeModifier { get; init; }
    
    [Inject]
    private LanguageTable? LangTable { get; init; }
    [Inject]
    private AppState? AppState { get; init; }

    private Color? HeroColour
    {
        get => _heroColour;
        set
        {
            if (value == _heroColour)
                return;
            
            _heroColour = value;
            StateHasChanged();
        }
    }

    private string? ColourString =>
        HeroColour == null ?
            null : $"--background-color: {HeroColour?.ToCssString()};";

    protected override async Task OnInitializedAsync()
    {
        LangTable!.LanguageChangedAsync += OnLanguageChangedAsync;
        await LangTable!.AwaitLanguageContentAsync(OnManifestLoadedAsync);
    }

    private async Task OnManifestLoadedAsync(object sender)
    {
        await SetHeroColour();
    }

    private async Task OnLanguageChangedAsync(object sender, int newLangIndex)
    {
        await SetHeroColour();
    }

    private async Task SetHeroColour()
    {
        if (HeroName is not null)
            HeroColour = (await LangTable!.LoadHeroPageInfo(HeroName!, AppState!.CurrentLanguage))?.ThemeColour;
        StateHasChanged();
    }

    public void Dispose()
    {
        LangTable!.LanguageChangedAsync -= OnLanguageChangedAsync;
        LangTable.ManifestLoadedAsync -= OnManifestLoadedAsync;
        GC.SuppressFinalize(this);
    }
}