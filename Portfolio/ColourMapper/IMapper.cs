using System.Drawing;

namespace Portfolio.ColourMapper;

public interface IMapper
{
    public Color? GetColour(int value);
}