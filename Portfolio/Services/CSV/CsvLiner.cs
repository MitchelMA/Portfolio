using System.Text;
using Portfolio.Configuration;
using Portfolio.Enums;
using Portfolio.Streams;

namespace Portfolio.Services.CSV;

internal class CsvLiner
{
    private int _lastChar = 0;
    private readonly StringBuilder _lineBuffer = new();
    private readonly List<string> _lines = new();
    private LexModi _currentMode = LexModi.Default;
    private readonly CsvSettings _settings;

    // This class is borrowing this "stream", so it is not allowed to dispose it
    private readonly StrippedStream _sStream;

    private readonly Dictionary<LexModi, Func<bool>> _modi = new();

    internal CsvLiner(StrippedStream stream, CsvSettings settings)
    {
        _sStream = stream;
        _settings = settings;
        ModiPopulator();
    }

    private void ModiPopulator()
    {
        _modi.Clear();

        _modi.Add(LexModi.Default, DefaultHandler);
        _modi.Add(LexModi.String, StringHandler);
        _modi.Add(LexModi.Comment, CommentHandler);
    }

    internal string[] GetLines()
    {
        while (_lastChar != -1)
        {
            _modi[_currentMode]();
        }

        return _lines.ToArray();
    }

    private bool _EOLProc()
    {
        string trimmed = _lineBuffer.ToString().Trim();
        if (_settings.Patches && trimmed.Length == 0)
            return false;

        _lines.Add(trimmed);
        _lineBuffer.Clear();
        return true;
    }

    private bool DefaultHandler()
    {
        while ((_lastChar = _sStream.ReadByte()) != -1)
        {
            if (_lastChar == _settings.CommentStarter)
            {
                _currentMode = LexModi.Comment;
                return true;
            }

            switch (_lastChar)
            {
                case '"':
                    _currentMode = LexModi.String;
                    return true;
                case '\n':
                    _EOLProc();
                    continue;
                default:
                    _lineBuffer.Append((char)_lastChar);
                    break;
            }
        }

        _EOLProc();

        return false;
    }

    private bool StringHandler()
    {
        _lineBuffer.Append((char)_lastChar);
        _lastChar = _sStream.ReadByte();
        if (_lastChar == '"')
            throw new Exception($"Cannot start string with two double-quotes `\"\"` at {_lineBuffer}");

        _lineBuffer.Append((char)_lastChar);
        while ((_lastChar = _sStream.ReadByte()) != -1)
        {
            if (_lastChar == '"')
            {
                _lineBuffer.Append((char)_lastChar);
                int t = _sStream.Peek();

                if (t == -1)
                {
                    _EOLProc();
                    _lastChar = -1;
                    return false;
                }

                if (t != '"')
                {
                    _currentMode = LexModi.Default;
                    return true;
                }

                _lastChar = _sStream.ReadByte();
            }

            _lineBuffer.Append((char)_lastChar);
        }

        throw new Exception($"String never ended with `\"` at {_lineBuffer}");
    }

    private bool CommentHandler()
    {
        // keep consuming till the end of the line
        while ((_lastChar = _sStream.ReadByte()) is not '\n' and not -1) ;
        switch (_lastChar)
        {
            case '\n':
                _EOLProc();
                break;

            case -1:
                _EOLProc();
                return false;
        }

        _currentMode = LexModi.Default;
        return true;
    }
}