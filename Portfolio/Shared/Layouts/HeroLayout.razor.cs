using Microsoft.AspNetCore.Components;
using Portfolio.Model.Hero;
using Portfolio.Services;

namespace Portfolio.Shared.Layouts;

public partial class HeroLayout
{
    [Inject]
    private AppState? AppState { get; init; }

    private HeroPageInfo? _heroInfo;

    public HeroPageInfo? HeroInfo
    {
        get => _heroInfo;
        set
        {
            _heroInfo = value;
            StateHasChanged();
        }
    }

}