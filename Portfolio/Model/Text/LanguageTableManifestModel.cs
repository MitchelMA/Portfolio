using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct LanguageTableManifestModel
{
   /// <summary>
   /// This field contains the Names of the specified cultures: "en-GB" or "nl-NL"
   /// </summary>
   [JsonPropertyName("language_index-table")]
   public string[] LanguageIndexTable { get; init; }
   
   [JsonPropertyName("header_filename")]
   public string HeaderFileName { get; init; }
   [JsonPropertyName("link-data_filename")]
   public string LinkDataFileName { get; init; }
   [JsonPropertyName("page-contents_prefix")]
   public string PageContentsPrefix { get; init; }
}