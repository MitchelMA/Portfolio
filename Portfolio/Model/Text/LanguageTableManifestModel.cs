using System.Text.Json.Serialization;

namespace Portfolio.Model.Text;

public struct LanguageTableManifestModel
{
   /// <summary>
   /// This field contains the Names of the specified cultures: ex. "en-GB" or "nl-NL"
   /// </summary>
   [JsonPropertyName("language_index-table")]
   public string[] LanguageIndexTable { get; init; }
   [JsonPropertyName("page-content-directory")]
   public string PageContentDirName { get; init; }
   [JsonPropertyName("header_filename")]
   public string HeaderFileName { get; init; }
   [JsonPropertyName("link-data_filename")]
   public string LinkDataFileName { get; init; }
   [JsonPropertyName("page-contents_prefix")]
   public string PageContentsPrefix { get; init; }
   [JsonPropertyName("other-content-directory")]
   public string OtherContentDirName { get; init; }
   [JsonPropertyName("info_table-filename")]
   public string InfoTableFileName { get; init; }
   [JsonPropertyName("duration-type-filename")]
   public string DurationTypeFilename { get; init; }
}