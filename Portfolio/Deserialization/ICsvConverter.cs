namespace Portfolio.Deserialization;

public interface ICsvConverter
{
    public object Read(string text);
}