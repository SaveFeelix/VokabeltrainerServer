#nullable disable
using REST_API.Data.Api.Dto.Vocable;

namespace REST_API.Data.Api.Dto.VocableCollection;

public class VocableCollectionGetDto
{
    public VocableCollectionGetDto()
    {
    }

    public VocableCollectionGetDto(int id, string name, IEnumerable<VocableGetDto> vocables)
    {
        Id = id;
        Name = name;
        Vocables = vocables.ToList();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public virtual IList<VocableGetDto> Vocables { get; set; } = new List<VocableGetDto>();
}