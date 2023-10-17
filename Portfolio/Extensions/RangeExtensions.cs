namespace Portfolio.Extensions;

public static class RangeExtensions
{
    public static bool Overlaps(this Range range, Range other)
    {
        return !(range.Start.Value < other.End.Value && other.Start.Value < range.End.Value);
    }
    
    public static bool Overlaps(this IEnumerable<Range> ranges)
    {
        var rangeArray = ranges as Range[] ?? ranges.ToArray();
        for (var i = 0; i < rangeArray.Length; i++)
        {
            var lead = rangeArray[i];
            for (var j = 0; j < rangeArray.Length; j++)
            {
                if (i == j)
                    continue;
                
                var check = rangeArray[j];
                if (lead.Overlaps(check)) return true;
            }
        }

        return false;
    }
}