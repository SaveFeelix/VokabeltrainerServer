#nullable disable
namespace REST_API.Data.Api.Dto.VocableCollection;

public class VocableCollectionCreateDto
{
    public VocableCollectionCreateDto()
    {
    }

    public VocableCollectionCreateDto(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}