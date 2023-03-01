namespace REST_API.Data.Api.Dto.Vocable;

public class VocableCreateDto
{
    public VocableCreateDto()
    {
    }

    public VocableCreateDto(int collectionId, string display, string possibleAnswers)
    {
        CollectionId = collectionId;
        Display = display;
        PossibleAnswers = possibleAnswers;
    }

    public int CollectionId { get; set; }
    public string Display { get; set; }
    public string PossibleAnswers { get; set; }
}