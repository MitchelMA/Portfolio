using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Portfolio.Enums;

[Obsolete("ProjectTags should be used instead")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectStatus
{
    [EnumMember(Value = "finished")]
    Finished,
    [EnumMember(Value = "in-development")]
    InDevelopment,
    [EnumMember(Value = "prototype")]
    Prototype,
}