using Portfolio.Model;
using Portfolio.Model.Text;

namespace Portfolio.Mappers;

public class ToNewHeaderDataMapper : IMapper<NewProjectMetaDataModel, HeaderData>
{
    public HeaderData MapFrom(NewProjectMetaDataModel obj)
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
        if (obj is not NewProjectMetaDataModel typedObj)
            return default;

        return MapFrom(typedObj);
    }
}