using System.Drawing;
using Portfolio.Deserialization;

namespace Portfolio.Converters.CSV;

public class CsvColorConverter : ICsvConverter<Color>
{
    private static readonly Color DefaultValue = Color.WhiteSmoke;
    public Color Read(string text)
    {
        if (text.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            return DefaultValue;
        
        if (text[0].Equals('#'))
            return ColorTranslator.FromHtml(text);
        
        var textSplits = text.Split(',', ';');
        var splitCount = textSplits.Length;

        if (splitCount <= 1)
            return Color.FromName(text);
        
        var intSplits = new int[splitCount];
        for (var i = 0; i < splitCount; i++)
            intSplits[i] = Convert.ToInt32(textSplits[i]);

        return intSplits.Length switch
        {
            < 3 => DefaultValue,
            3 => Color.FromArgb(intSplits[0], intSplits[1], intSplits[2]),
            _ => Color.FromArgb(intSplits[3], intSplits[0], intSplits[1], intSplits[2])
        };
    }

    object ICsvConverter.Read(string text)
    {
        return Read(text);
    }
}