using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfolio.Converters.JSON;

public class JsonColorConverter : JsonConverter<Color>
{
    private static readonly Color DefaultValue = Color.WhiteSmoke;
    
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString()!;
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

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}