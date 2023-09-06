using Portfolio.Configuration;
using Portfolio.Deserialization;
using Portfolio.Streams;

namespace Portfolio.Services.CSV;

public class CsvLexer : IDisposable
{
    private readonly FileInfo? _file;
    private readonly CsvSettings _settings = CsvSettings.Default;

    private readonly StrippedStream _sStream;

    private string[]? _lines;
    private string[][]? _splits;
    private string[]? _header;

    public string[]? Lines => _lines;
    public string[][]? Splits => _splits;
    public string[]? Header => _header;

    public bool IsDisposed { get; private set; } = false;

    public CsvLexer(string fileText)
    {
        _sStream = new(fileText);
    }

    public CsvLexer(FileInfo file)
    {
        if (!file.Exists)
            throw new FileNotFoundException($"File {file.FullName} does not exist");

        _file = file;
        _sStream = new(File.ReadAllText(file.FullName));
    }

    public CsvLexer(string fileText, CsvSettings settings) : this(fileText)
    {
        _settings = settings;
    }

    public CsvLexer(FileInfo file, CsvSettings settings) : this(file)
    {
        _settings = settings;
    }

    private string[] GetLines()
    {
        CsvLiner liner = new(_sStream, _settings);
        string[] lines = liner.GetLines();
        _sStream.Reset();

        return lines;
    }

    public string[][] Lex()
    {
        if (_splits is not null)
            return _splits;

        _lines = GetLines();
        using CsvSplitter splitter = new(_lines, _settings);
        _splits = splitter.Split();

        _header = splitter.Header;
        return _splits;
    }

    public T[] Deserialize<T>() where T : new()
    {
        if (!_settings.FirstIsHeader)
            throw new Exception("FirstIsHeader needs to be set to `True` in the settings for deserialization");

        Lex();
        CsvDeserializer<T> deserializer = new(_header!, _splits!);
        return deserializer.Deserialize();
    }

    public T[] Deserialize<T>(string[] headers, bool ignoreFirst = false) where T : new()
    {
        Lex();
        string[][] splits = ignoreFirst ? _splits!.Skip(2).ToArray() : _splits!;

        CsvDeserializer<T> deserializer = new(headers, splits);

        return deserializer.Deserialize();
    }

    public Task<string[][]> LexAsync()
    {
        return Task.Run(Lex);
    }

    public Task<T[]> DeserializeAsync<T>() where T : new()
    {
        return Task.Run(Deserialize<T>);
    }

    public Task<T[]> DeserializeAsync<T>(string[] headers, bool ignoreFirst = false) where T : new()
    {
        return Task.Run(() => Deserialize<T>(headers, ignoreFirst));
    }

    #region IDisposable pattern

    private void ReleaseManagedResources()
    {
        _sStream.Dispose();
    }

    private void ReleaseUnmanagedResources()
    {
        _lines = null;
        _splits = null;
        _header = null;
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            ReleaseManagedResources();
        }

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CsvLexer()
    {
        Dispose(false);
    }

    #endregion
}