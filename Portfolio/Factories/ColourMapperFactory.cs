using System.Drawing;
using Portfolio.ColourMapper;
using Portfolio.Services;

namespace Portfolio.Factories;

public class ColourMapperFactory
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<Type, IMapper> _maps = new();

    public ColourMapperFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Mapper<T>?> CreateMap<T>(string fileHref = "")
        where T : Enum
    {
        if (_maps.ContainsKey(typeof(T)) || fileHref.Length == 0)
            return _maps[typeof(T)] as Mapper<T>;

        var fileText = await _httpClient.GetStringAsync(fileHref);
        var outcome = ToDict<T>(fileText)!;

        if (outcome.Count == 0)
            return null;

        var mapper = new Mapper<T>(outcome);
        _maps.Add(typeof(T), mapper);
        return mapper;
    }

    private static Dictionary<T, Color>? ToDict<T>(string fileText)
        where T : Enum
    {
        string[][]? values = CsvCommentLexer.LexValues(fileText);
        int l = values?.Length ?? 0;
        if (l == 0)
            return null;

        Dictionary<T, Color> dict = new();

        for (int i = 0; i < l; i++)
        {
            var cur = values![i];
            T value = (T)(object)int.Parse(cur[0]);
            int[] rgba = cur[1].Split(' ').Select(int.Parse).ToArray();
            Color color = Color.FromArgb(rgba[3], rgba[0], rgba[1], rgba[2]);
            dict.Add(value, color);
        }
        
        return dict;
    }

    public void Flush()
    {
        _maps.Clear();
    }
}