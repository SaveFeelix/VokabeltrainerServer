namespace REST_API.Data.Api.Dto.VocableCollection;

public class VocableCollectionUpdateDto
{
    public VocableCollectionUpdateDto()
    {
    }

    public VocableCollectionUpdateDto(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}