using System.Text;

namespace Portfolio.Model.Tags;

public class LinkTag
{
    public Dictionary<string, object>? Attributes;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<link ");

        if (Attributes is not null)
        {
            foreach (var item in Attributes)
            {
                sb.Append($"{item.Key}=\"{item.Value?.ToString()}\" ");
            }
        }

        sb.Append("/>");
        return sb.ToString();
    }
}
