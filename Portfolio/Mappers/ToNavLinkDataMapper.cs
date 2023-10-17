using Portfolio.Enums;
using Portfolio.Model;
using Portfolio.Model.Text;

namespace Portfolio.Mappers;

public class ToNavLinkDataMapper : IMapper<LangLinkModel, NavLinkData>, IMapper
{
    public NavLinkData MapFrom(LangLinkModel obj)
    {
        return new NavLinkData(obj.Href, obj.DisplayText, obj.NavigationType ?? NavigationType.Stays);
    }

    public object? MapFrom(object? obj)
    {
        if (obj is not LangLinkModel typedObj)
            return default;

        return MapFrom(typedObj);
    }
}