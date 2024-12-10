using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio.Model.Tags;

namespace Portfolio.Converters.JSON;

public class PageIconConverter : JsonConverter<PageIcon>
{
    public override PageIcon? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value == null)
            return null;

        var splits = value.Split('|');
        return new PageIcon(splits[0], splits[1]);
    }

    public override void Write(Utf8JsonWriter writer, PageIcon value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}