using System.ComponentModel;

namespace Portfolio.Deserialization;

internal static class Converter
{
    /// <summary>
    /// Converts from string "value" to an instance of Type T
    /// </summary>
    /// <param name="value">input string</param>
    /// <typeparam name="T">output Type</typeparam>
    /// <returns></returns>
    internal static T ConvertTo<T>(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        T result = (T)converter.ConvertFromString(value)!;

        return result;
    }

    internal static object ConvertToType(Type type, string value)
    {
        var converter = TypeDescriptor.GetConverter(type);
        object result = converter.ConvertFromString(value)!;

        return result;
    }
}