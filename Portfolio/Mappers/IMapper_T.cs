namespace Portfolio.Mappers;

public interface IMapper<in T, out T2>
{
    public abstract T2 MapFrom(T obj);
}