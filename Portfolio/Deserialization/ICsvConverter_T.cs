namespace Portfolio.Deserialization;

public interface ICsvConverter<out T> : ICsvConverter
{
    public new T Read(string text);
}