using System.Drawing;

namespace Portfolio.Services;

public class ColourMapper<T>
    where T : Enum
{
    private readonly Dictionary<T, Color> _values;
    
        public ColourMapper(Dictionary<T, Color> values)
        {
            _values = values;
        }
    
        public (bool succes, Color? color) GetColour(T value)
        {
            if (_values.ContainsKey(value))
                return (true, _values[value]);
    
            return (false, null);
        }
}