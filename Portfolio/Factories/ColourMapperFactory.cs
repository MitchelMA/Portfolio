using System.Drawing;
using Portfolio.ColourMapper;

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
        var outcome = Lexer<T>(fileText)!;

        if (outcome is null || outcome.Count == 0)
            return null;

        var mapper = new Mapper<T>(outcome);
        _maps.Add(typeof(T), mapper);
        return mapper;
    }

    private Dictionary<T, Color>? Lexer<T>(string fileText)
        where T : Enum
    {
        string trim = fileText.Trim();
        if (trim.Length == 0)
            return null;

        Dictionary<T, Color> dict = new();

        trim.ReplaceLineEndings();
        Span<string> lines = trim.Split(Environment.NewLine);
        int l = lines.Length;
        for (int i = 0; i < l; i++)
        {
            var cur = lines[i];

            string[] split = cur.Split(',');
            int[] colourValues = split[1].Trim().Split(' ').Select(int.Parse).ToArray();

            T value = (T)(object)int.Parse(split[0]);
            Color color = Color.FromArgb(colourValues[3], colourValues[0], colourValues[1], colourValues[2]);
            dict.Add(value, color);
        }

        return dict;
    }

    public void Flush()
    {
        _maps.Clear();
    }
}