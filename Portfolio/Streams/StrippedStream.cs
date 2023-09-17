using System.Text;

namespace Portfolio.Streams;

internal class StrippedStream : IDisposable
{
    private readonly IEnumerator<byte> _bytes;
    private int _lastValue;
    private int _nextValue;

    internal bool IsDisposed { get; private set; } = false;

    internal StrippedStream(IEnumerable<byte> byteCollection)
    {
        _bytes = byteCollection.GetEnumerator();
        // the first setup of the values to proceed reading further
        StartProc();
    }

    internal StrippedStream(string text) : this(Encoding.UTF8.GetBytes(text))
    {
    }

    private void StartProc()
    {
        _bytes.MoveNext();
        _nextValue = _bytes.Current;
        ReadValues();
    }

    private bool ReadValues()
    {
        _lastValue = _nextValue;
        _nextValue = _bytes.MoveNext() ? _bytes.Current : -1;

        return true;
    }

    internal int ReadByte()
    {
        int val = _lastValue;
        ReadValues();
        return val;
    }

    internal int Peek() => _lastValue;

    internal void Reset()
    {
        _lastValue = 0;
        _nextValue = 0;
        _bytes.Reset();
    }

    #region IDisposable Pattern

    private void ReleaseManagedResources()
    {
        _bytes.Dispose();
    }

    private void ReleaseUnmanagedResources()
    {
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

    ~StrippedStream()
    {
        Dispose(false);
    }

    #endregion
}