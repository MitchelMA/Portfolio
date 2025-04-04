using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portfolio.Converters.JSON;

public class JsonColourConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return ColorTranslator.FromHtml(value!);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ColorTranslator.ToHtml(value));
    }
}