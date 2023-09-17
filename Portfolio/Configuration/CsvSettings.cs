namespace Portfolio.Configuration;

public struct CsvSettings
{
    public bool FirstIsHeader;
    public char Separator;
    public char? CommentStarter;
    public bool Patches;

    public static CsvSettings Default => new()
    {
        FirstIsHeader = false,
        Separator = ',',
        CommentStarter = null,
        Patches = false,
    };
}