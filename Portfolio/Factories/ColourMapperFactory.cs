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

    /// <summary>
    /// Creates (or gets from the already created maps) a colour-map from the given filepath to a csv file
    /// in the syntax: `{enum-idx}, {r} {g} {b} {a}`. With support of comments starting with `#`. <br />
    /// In the situation where `upperBound` is `0`, the csv file will be lexed starting from `lowerBound`!
    /// </summary>
    /// <param name="fileHref">path to the csv file in wwwroot</param>
    /// <param name="lowerBound">Lower inclusive bound of the lexed lines</param>
    /// <param name="upperBound">Upper exclusive bound of the lexed lines</param>
    /// <typeparam name="T">Any kind of enum</typeparam>
    /// <returns>A colour-mapper created from the specified csv file</returns>
    public async Task<Mapper<T>?> CreateMap<T>(string fileHref = "", int lowerBound = 0, int upperBound = 0)
        where T : Enum
    {
        if (fileHref.Length == 0 || _maps.ContainsKey(typeof(T)))
            return _maps[typeof(T)] as Mapper<T>;

        var fileText = await _httpClient.GetStringAsync(fileHref);
        var outcome = ToDict<T>(fileText, lowerBound, upperBound)!;

        if (outcome.Count == 0)
            return null;

        var mapper = new Mapper<T>(outcome);
        _maps.Add(typeof(T), mapper);
        return mapper;
    }

    /// <summary>
    /// Forces the skipping of checking in the already created maps and **ALWAYS** creates a new one from the
    /// input csv file.
    /// </summary>
    /// <param name="fileHref"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<Mapper<T>?> ForceNew<T>(string fileHref, int lowerBound = 0, int upperBound = 0)
        where T : Enum
    {
        var fileText = await _httpClient.GetStringAsync(fileHref);
        var outcome = ToDict<T>(fileText, lowerBound, upperBound)!;

        if (outcome.Count == 0)
            return null;

        var mapper = new Mapper<T>(outcome);
        var key = typeof(T);
        if (_maps.ContainsKey(key))
        {
            _maps[key] = mapper;
        }
        else
        {
            _maps.Add(key, mapper);
        }

        return mapper;
    }

    private static Dictionary<T, Color>? ToDict<T>(string fileText, int lowerBound = 0, int upperBound = 0)
        where T : Enum
    {
        string[][]? values = CsvCommentLexer.LexValues(fileText, lowerBound, upperBound);
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