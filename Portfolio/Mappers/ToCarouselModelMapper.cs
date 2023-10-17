using Portfolio.Model;
using Portfolio.Model.Project;

namespace Portfolio.Mappers;

public class ToCarouselModelMapper : IMapper<ProjectDataModel, CarouselModel>, IMapper
{
    public CarouselModel MapFrom(ProjectDataModel obj)
    {
        return new CarouselModel
        {
            Href = obj.LocalHref,
            SetHeight = obj.CardInfo.SetHeight,
            Background = obj.Header.HeaderImage,
            Tags = obj.Tags,
        };
    }

    public object? MapFrom(object? obj)
    {
        if (obj is not ProjectDataModel typedObj)
            return default;

        return MapFrom(typedObj);
    }
}