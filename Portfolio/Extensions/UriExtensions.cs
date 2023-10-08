using System.Text;

namespace Portfolio.Extensions;

public static class UriExtensions
{
    public static Dictionary<string, string>? ParseQuery(this Uri uri)
    {
        var queryString = uri.Query;
        if (queryString.Length == 0)
            return default;
        queryString = queryString[1..];

        var dict = new Dictionary<string, string>();
        var splits = queryString.Split('&');
        foreach (var split in splits)
        {
            var innerSplits = split.Split('=');
            var key = innerSplits[0];
            var value = innerSplits[1];
            dict.Add(key, value);
        }
        return dict;
    }

    public static string AddQuery(this Uri uri, string queryName, string value)
    {
        var parsed = uri.ParseQuery() ?? new Dictionary<string, string>();

        if (!parsed.TryAdd(queryName, value))
            return uri.AbsoluteUri;
        
        return uri.AbsoluteUri + $"&{Uri.EscapeDataString(queryName)}={Uri.EscapeDataString(value)}";
    }

    public static string SetQuery(this Uri uri, string queryName, string value)
    {
        var parsed = uri.ParseQuery() ?? new Dictionary<string, string>();

        parsed[queryName] = value;
        var basePath = uri.AbsoluteUri.Split('?')[0];
        var queryBuilder = new StringBuilder();
        foreach (var query in parsed)
        {
            queryBuilder.Append(Uri.EscapeDataString(query.Key));
            queryBuilder.Append('=');
            queryBuilder.Append(Uri.EscapeDataString(query.Value));
        }
        return basePath + $"?{queryBuilder}";
    }
}