using System.Drawing;

namespace Portfolio.Extensions;

public static class ColorExtensions
{
    public static string ToCssString(this Color color) =>
        $"rgb({color.R} {color.G} {color.B} / {color.A / 255f * 100}%)";
}