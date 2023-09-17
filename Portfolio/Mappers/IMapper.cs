namespace Portfolio.Mappers;

public interface IMapper<in T, out T2>
{
    public static T2 MapFrom(T obj)
    {
        return default!;
    }
}