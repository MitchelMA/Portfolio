using Portfolio.Deserialization;

namespace Portfolio.Configuration;

public struct CsvDeserializationOptions
{
    public readonly List<ICsvConverter>? Converters;
    
    public CsvDeserializationOptions(List<ICsvConverter> converters)
    {
        Converters = converters;
    }
}