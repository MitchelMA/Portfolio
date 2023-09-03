using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace Portfolio.Extensions;

public static class ColorExtensions
{
    private static string GetPercentageString(byte alpha) =>
        (alpha / 255f * 100).ToString(CultureInfo.InvariantCulture);

    public static string ToCssString(this Color color) =>
        $"rgb({color.R} {color.G} {color.B} / {GetPercentageString(color.A)}%)";
}