namespace Portfolio.Services;

[Obsolete("This class is obsolute and should not be used")]
public class LangTablePreCacher
{
    private string[]? _extra;
    private LanguageTable _langTable;
    private ProjectInfoGetter _infoGetter;

    public string[] Extra
    {
        get => _extra ??= Array.Empty<string>();
        set => _extra = value;
    }
    
    public LangTablePreCacher(LanguageTable table, ProjectInfoGetter infoGetter)
    {
        _langTable = table;
        _infoGetter = infoGetter;
    }

    public async Task PreCache(int langCode)
    {
        await _infoGetter.RetrieveData();
        var uris = _infoGetter.Data.Keys;
        uris = uris.Concat(Extra);

        _langTable.PreCacheAll(uris, langCode);
    }
}