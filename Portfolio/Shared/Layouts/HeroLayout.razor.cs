using Microsoft.AspNetCore.Components;
using Portfolio.Services;

namespace Portfolio.Shared.Layouts;

public partial class HeroLayout
{
    [Inject]
    private AppState? AppState { get; init; }
}