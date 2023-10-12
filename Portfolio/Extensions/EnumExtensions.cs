using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Portfolio.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<TEnum> ExtractFlags<TEnum>(this TEnum value)
        where TEnum : IConvertible, IComparable
    {
        var underlyingType = value.GetType();
        if (!underlyingType.IsEnum)
            throw new ArgumentException($"Parameter `{nameof(value)}` was not of type Enum");
        if (underlyingType.GetCustomAttribute<FlagsAttribute>() is null)
            throw new ArgumentException($"Parameter `{nameof(value)}` Enum missing attribute [Flags]");

        List<TEnum> individualFlags = new();
        var bitCount = Unsafe.SizeOf<TEnum>() * 8;

        for (var bitNumber = 0; bitNumber < bitCount; bitNumber++)
        {
            var filter = 1 << bitNumber;
            var val = (int) (object) value & filter;
            if (val > 0) individualFlags.Add((TEnum) (object) val);
        }

        return individualFlags;
    }

    public static string ToMemberString<TEnum>(this TEnum value)
        where TEnum : IConvertible, IComparable
    {
        var underlyingType = value.GetType();
        if (!underlyingType.IsEnum)
            throw new ArgumentException($"Parameter `{nameof(value)}` was not of type Enum");
        
        var memberInfo = underlyingType.GetMember(value.ToString()!);
        if (memberInfo.Length == 0)
            return value.ToString() ?? string.Empty;
        
        var memberAttr = memberInfo[0].GetCustomAttribute<EnumMemberAttribute>();
        if (memberAttr is null)
            return value.ToString() ?? string.Empty;

        return memberAttr.Value ?? value.ToString() ?? string.Empty;
    }

    public static string ToMemberStrings<TEnums>(this TEnums value)
        where TEnums : IConvertible, IComparable
    {
        var indFlags = value.ExtractFlags() as List<TEnums>;
        var sb = new StringBuilder();
        var flagCount = indFlags?.Count ?? 0;

        for (var i = 0; i < flagCount; i++)
        {
            var flag = indFlags![i];
            sb.Append(flag.ToMemberString());
            if (i < flagCount - 1)
                sb.Append(", ");
        }

        return sb.ToString();
    }
}