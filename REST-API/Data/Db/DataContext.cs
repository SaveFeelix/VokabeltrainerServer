#nullable disable
using Microsoft.EntityFrameworkCore;
using REST_API.Data.Db.Models;
using REST_API.Data.Db.Models.Vocable;

namespace REST_API.Data.Db;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<VocableCollection> VocableCollections { get; set; }
    public DbSet<Vocable> Vocables { get; set; }
}