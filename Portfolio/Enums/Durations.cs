using System.Text.Json.Serialization;

namespace Portfolio.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Durations
{
    Day,
    Week,
    Month,
    Year,
    Decade
}