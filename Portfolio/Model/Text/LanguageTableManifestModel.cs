using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct LanguageTableManifestModel
{
   [JsonPropertyName("LanguageIndexTable")]
   public string[] LanguageIndexTable { get; init; }
   
   [JsonPropertyName("HeaderFileNamePrefix")]
   public string HeaderFileNamePrefix { get; init; }
}