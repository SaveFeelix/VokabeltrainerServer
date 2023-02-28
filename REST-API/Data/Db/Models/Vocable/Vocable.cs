namespace REST_API.Data.Db.Models.Vocable;

public class Vocable
{
    public int Id { get; set; }
    public string Display { get; set; }
    public string PossibleAnswers { get; set; }
    
    public virtual VocableCollection Collection { get; set; }
}