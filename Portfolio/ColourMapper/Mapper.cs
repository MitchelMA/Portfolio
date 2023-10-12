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
        if (_values.TryGetValue(value, out var colour))
            return colour;

        return null;
    }

    public Color? GetColour(int value)
    {
        // Nice
        var val = (T)(object)value;
        return GetColour(val);
    }
}