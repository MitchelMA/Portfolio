using Portfolio.Model;
using Portfolio.Model.Project;

namespace Portfolio.Mappers;

public class ToCarouselModelMapper : IMapper<ProjectDataModel, CarouselModel>
{
    public static CarouselModel MapTo(ProjectDataModel obj)
    {
        return new CarouselModel
        {
            Href = obj.LocalHref,
            SetHeight = obj.CardInfo.SetHeight,
            Background = obj.Header.HeaderImage,
            Tags = obj.Tags,
        };
    }
}