
using System.Text.Json.Serialization;

namespace Portfolio.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NavigationType
{
    Stays = 0,
    Internal,
    External
}