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
    [JsonPropertyName("other-content-directory")]
    public string OtherContentDirName { get; init; }
    [JsonPropertyName("hero-content-directory")]
    public string HeroContentDirName { get; init; }
    
    [JsonPropertyName("page-islands-text-location")]
    public string PageIslandsTextLocation { get; init; }

    [JsonPropertyName("header-filename")]
    public string HeaderFileName { get; init; }
    [JsonPropertyName("link-data-filename")]
    public string LinkDataFileName { get; init; }
    [JsonPropertyName("info-table-filename")]
    public string InfoTableFileName { get; init; }
    [JsonPropertyName("duration-type-filename")]
    public string DurationTypeFilename { get; init; }

    [JsonPropertyName("page-contents-prefix")]
    public string PageContentsPrefix { get; init; }

    [JsonPropertyName("hero-contents-prefix")]
    public string HeroContentsPrefix { get; init; }
}