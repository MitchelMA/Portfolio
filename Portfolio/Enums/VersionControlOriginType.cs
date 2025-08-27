using System.Text.Json.Serialization;

namespace Portfolio.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VersionControlOriginType
{
    GitHub,
    GitLab,
}