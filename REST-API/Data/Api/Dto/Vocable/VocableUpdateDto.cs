namespace REST_API.Data.Api.Dto.Vocable;

public class VocableUpdateDto
{
    public VocableUpdateDto()
    {
    }

    public VocableUpdateDto(int id, string display, string possibleAnswers)
    {
        Id = id;
        Display = display;
        PossibleAnswers = possibleAnswers;
    }

    public int Id { get; set; }
    public string Display { get; set; }
    public string PossibleAnswers { get; set; }
}