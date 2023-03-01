#nullable disable
namespace REST_API.Data.Db.Models.Vocable;

public class VocableCollection
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual IList<Vocable> Vocables { get; set; }

    public virtual User Owner { get; set; }
}