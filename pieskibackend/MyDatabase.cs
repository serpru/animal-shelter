using Microsoft.EntityFrameworkCore;
using pieskibackend;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries;

namespace TodoApi.Models;

public class MyDatabase : DbContext
{
    public MyDatabase(DbContextOptions<MyDatabase> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder o)
    {
        o.UseSqlServer("Data Source=localhost;Initial Catalog=animal_shelter;Integrated Security=True;TrustServerCertificate=True");
    }
    public DbSet<Animal> Animal { get; set; } = null!;
    public DbSet<AnimalSpecies> AnimalSpecies { get; set; } = null!;
    public DbSet<Aggression> Aggression { get; set; } = null!;
    public DbSet<AnimalImage> AnimalImage { get; set; } = null!;
    public DbSet<Breed> Breed { get; set; } = null!;
    public DbSet<Origin> Origin { get; set; } = null!;
    public DbSet<Size> Size { get; set; } = null!;
    public DbSet<Status> Status { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;
}