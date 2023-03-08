using System.Drawing;

namespace Portfolio.ColourMapper;

public class Mapper<T> : IMapper
    where T : Enum
{
    private readonly Dictionary<T, Color> _values;

    public Mapper(Dictionary<T, Color> values)
    {
        _values = values;
    }

    public Color? GetColour(T value)
    {
        if (_values.ContainsKey(value))
            return _values[value];

        return null;
    }

    public Color? GetColour(int value)
    {
        // Nice
        T val = (T)(object)value;
        return GetColour(val);
    }
}