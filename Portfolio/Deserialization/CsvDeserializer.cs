using System.Reflection;
using System.Runtime.Serialization;
using Portfolio.Attributes;
using Portfolio.Configuration;

namespace Portfolio.Deserialization;

internal class CsvDeserializer<T> where T : new()
{
    private const BindingFlags Binds = BindingFlags.Instance | BindingFlags.Public;

    private readonly (PropertyInfo propInfo, CsvPropertyNameAttribute custAttr)[] _csvProps;
    private readonly Dictionary<string, Type> _typeMap;
    private readonly Type _tType;

    private string[] _headers;
    private string[][] _values;
    private CsvDeserializationOptions _options;
    private Dictionary<Type, ICsvConverter> _typedConverters = new();

    /// <summary>
    /// Creates a new instance of a CsvDeserializer for Generic T
    /// </summary>
    /// <param name="headers">The header values of the csv file</param>
    /// <param name="values">The comma-seperated-values of the csv excluding the header</param>
    /// <param name="options">The Deserialization options</param>
    /// <exception cref="SerializationException">
    /// When no properties with the attribute CsvPropertyName are in the target type
    /// </exception>
    /// <exception cref="MissingMemberException">
    /// When there is a missing property for a header value
    /// </exception>
    internal CsvDeserializer(string[] headers, string[][] values, CsvDeserializationOptions options = new())
    {
        _headers = headers;
        _values = values;
        _options = options;
        SetupTypedConverters();

        _tType = typeof(T);
        _csvProps = GetCsvProps();
        _typeMap = GetTypeMap();

        if (_csvProps.Length is 0)
            throw new SerializationException(
                $"No properties with {nameof(CsvPropertyNameAttribute)} found on target type `{typeof(T).FullName}`");

        if (_typeMap.Count != _csvProps.Length)
        {
            string[] missing = GetMissingMembers();
            string missingJ = string.Join(", ", missing);
            throw new MissingMemberException(
                $"Target type does not include property for every header value.\nMissing properties for header-values: `{missingJ}`");
        }
    }

    private void SetupTypedConverters()
    {
        if (_options.Converters is null)
            return;
        
        var convertersCount = _options.Converters.Count;
        for (var i = 0; i < convertersCount; i++)
        {
            var converterType = _options.Converters[i].GetType();
            var implementedInterfaces = converterType.GetInterfaces();
            var genericType = implementedInterfaces.First(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(ICsvConverter<>)).GetGenericArguments()[0];
            _typedConverters.Add(genericType, _options.Converters[i]);
        }
    }

    internal T[] Deserialize()
    {
        T[] ts = new T[_values.Length];
        int l = ts.Length;
        for (int i = 0; i < l; i++)
        {
            ts[i] = DeserializeLine(_values[i]);
        }

        return ts;
    }

    private T DeserializeLine(string[] lineValues)
    {
        object inst = new T();
        Span<KeyValuePair<string, Type>> typeMapS = _typeMap.ToArray();
        int l = typeMapS.Length;
        for (int i = 0; i < l; i++)
        {
            var cur = typeMapS[i];
            var foundConverter = _typedConverters.TryGetValue(cur.Value, out var specificConverter);

            var converted = foundConverter ? specificConverter!.Read(lineValues[i]) : Converter.ConvertToType(cur.Value, lineValues[i]);
            _tType.InvokeMember(cur.Key, Binds | BindingFlags.SetProperty, Type.DefaultBinder, inst,
                new[] { converted });
        }

        return (T)inst;
    }


    private Dictionary<string, Type> GetTypeMap()
    {
        Dictionary<string, Type> dict = new();
        Span<string> headersL = _headers;
        int l = _csvProps.Length;

        for (int i = 0; i < l; i++)
        {
            var cur = _csvProps[i];

            int propIdx = headersL.IndexOf(cur.propInfo.Name);
            if (cur.custAttr.Name is not null)
                propIdx = propIdx is -1 ? headersL.IndexOf(cur.custAttr.Name) : propIdx;


            if (propIdx != -1 && propIdx < _csvProps.Length)
                dict.Add(_csvProps[propIdx].propInfo.Name, _csvProps[propIdx].propInfo.PropertyType);
        }

        return dict;
    }

    private (PropertyInfo, CsvPropertyNameAttribute)[] GetCsvProps()
    {
        List<(PropertyInfo, CsvPropertyNameAttribute)> props = new();
        Span<PropertyInfo> all = _tType.GetProperties(Binds);

        int l = all.Length;

        for (int i = 0; i < l; i++)
        {
            var cur = all[i];
            var attr = cur.GetCustomAttribute<CsvPropertyNameAttribute>();
            if (attr is not null)
                props.Add((cur, attr));
        }

        return props.ToArray();
    }

    #region MissingHeaderFinding

    private string[] GetMissingMembers()
    {
        List<string> missing = new();
        Span<string> keyList = GetHeaderKeys();
        int l = _headers.Length;
        for (int i = 0; i < l; i++)
        {
            if (!keyList.Contains(_headers[i]))
                missing.Add(_headers[i]);
        }

        return missing.ToArray();
    }

    private string[] GetHeaderKeys()
    {
        Span<(PropertyInfo, CsvPropertyNameAttribute)> csvPropsS = _csvProps;
        List<string> keys = new();
        int l = csvPropsS.Length;
        for (int i = 0; i < l; i++)
        {
            var cur = csvPropsS[i];
            if (cur.Item2.Name is not null)
            {
                keys.Add(cur.Item2.Name);
                continue;
            }

            keys.Add(cur.Item1.Name);
        }

        return keys.ToArray();
    }

    #endregion // MissingHeaderFinding
}