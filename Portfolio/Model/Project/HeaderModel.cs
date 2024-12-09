using System.Text.Json.Serialization;
using Portfolio.Converters.JSON;
using Portfolio.Model.Tags;

namespace Portfolio.Model.Project;

public struct HeaderModel
{
    public string HeaderImage { get; init; }
    [JsonConverter(typeof(PageIconConverter))]
    public PageIcon? PageIcon { get; init; }
}