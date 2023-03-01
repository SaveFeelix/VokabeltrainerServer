#nullable disable
namespace REST_API.Data.Api.Dto.Vocable;

public class VocableGetDto
{
    public VocableGetDto(int id, string display, string possibleAnswers)
    {
        Id = id;
        Display = display;
        PossibleAnswers = possibleAnswers;
    }

    public int Id { get; }
    public string Display { get; }
    public string PossibleAnswers { get; }
}