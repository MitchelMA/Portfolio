using Portfolio.Model;
using Portfolio.Model.Text;

namespace Portfolio.Mappers;

public class ToHeaderDataMapper : IMapper<LangHeaderModel, HeaderData>, IMapper
{
    public HeaderData MapFrom(LangHeaderModel obj)
    {
        return new HeaderData
        {
            Title = obj.Title,
            UnderTitle = obj.UnderTitle,
            Description = obj.Description,
            ImagePath = null,
            ExtraStyles = null
        };
    }

    public object? MapFrom(object? obj)
    {
        if (obj is not LangHeaderModel typedObj)
            return default;

        return MapFrom(typedObj);
    }
}