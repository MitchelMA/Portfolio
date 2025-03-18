using Microsoft.AspNetCore.Components;
using Portfolio.Model.Hero;
using Portfolio.Services;

namespace Portfolio.Shared.Layouts;

public partial class HeroLayout
{
    [Inject]
    private AppState? AppState { get; init; }

    private NewHeroMeta? _heroInfo;

    public NewHeroMeta? HeroInfo
    {
        get => _heroInfo;
        set
        {
            _heroInfo = value;
            StateHasChanged();
        }
    }

}