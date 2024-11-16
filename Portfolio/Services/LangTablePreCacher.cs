namespace Portfolio.Services;

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

    public async Task<bool> PreCache(int langCode)
    {
        await _infoGetter.RetrieveData();
        var uris = _infoGetter.Data.Keys;
        uris = uris.Concat(Extra);

        return _langTable.PreCacheAll(uris, langCode);
    }
}