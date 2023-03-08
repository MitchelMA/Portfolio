using System.Text;

namespace Portfolio.Services;

public static class CsvCommentLexer
{
    public static char CommentStarter = '#';
    public static char CsvSplitter = ',';
    
    /// <summary>
    /// Lexes out the comments from the line with the specified `CommendStarter`
    /// </summary>
    /// <param name="csv">The CSV string you want to get lexed</param>
    /// <returns>An array of trimmed lines without the comments</returns>
    public static string[] LexComments(string csv)
    {
        byte[] byteArr = Encoding.UTF8.GetBytes(csv.Trim());
        using MemoryStream ms = new(byteArr);
        string buffer = string.Empty;
        List<string> lines = new();

        int c;

        // while ReadByte didn't reach the end, keep consuming the next byte and put it into `c`
        while ((c = ms.ReadByte()) != -1)
        {
            // when we find a comment-starter, keep consuming till the end of the line or EOF
            if (c == CommentStarter)
            {
                while (ms.ReadByte() is not '\n' and not -1) ;
                // after consumption, add the line to the list of lines
                lines.Add(buffer.Trim());
                buffer = string.Empty;
                continue;
            }

            // when we find the end of the line, add the buffer to the list of lines
            if (c == '\n')
            {
                lines.Add(buffer.Trim());
                buffer = string.Empty;
                continue;
            }

            // add char `c` to the buffer
            buffer += (char)c;
        }

        // add the last buffer to the list of lines to include the last line
        if (buffer.Length > 0)
            lines.Add(buffer.Trim());

        return lines.ToArray();
    }

    /// <summary>
    /// Lexes the csv string fully, also ignoring comments
    /// </summary>
    /// <param name="csv">The CSV string</param>
    /// <returns>an array of the split csv values</returns>
    public static string[][]? LexValues(string csv)
    {
        Span<string> lines = LexComments(csv);
        int l = lines.Length;
        
        if (l == 0)
            return null;

        string[][] data = new string[l][];

        for (int i = 0; i < l; i++)
        {
            data[i] = lines[i].Split(CsvSplitter).Select(x => x.Trim()).ToArray();
        }

        return data;
    }
}