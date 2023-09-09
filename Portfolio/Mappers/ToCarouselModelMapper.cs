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
            Background = obj.Header.HeaderImage,
            
        };
    }
}