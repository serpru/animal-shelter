using Microsoft.EntityFrameworkCore;
using pieskibackend;
using pieskibackend.Api.Requests;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;

namespace TodoApi.Models;

public class MyDatabase : DbContext
{
    public MyDatabase(DbContextOptions<MyDatabase> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder o)
    {
        if (!o.IsConfigured)
        {
            o.UseSqlServer("Data Source=localhost;Initial Catalog=animal_shelter;Integrated Security=True;TrustServerCertificate=True");
        }
    }
    public DbSet<Adoptee> Adoptee { get; set; } = null!;
    public DbSet<AdoptionEvent> AdoptionEvent { get; set; } = null!;
    public DbSet<AdoptionStatus> AdoptionStatus { get; set; } = null!;
    public DbSet<Animal> Animal { get; set; } = null!;
    public DbSet<AnimalAddForm> AnimalAddForm { get; set; } = null!;
    public DbSet<AnimalSpecies> AnimalSpecies { get; set; } = null!;
    public DbSet<Aggression> Aggression { get; set; } = null!;
    public DbSet<AnimalImage> AnimalImage { get; set; } = null!;
    public DbSet<Breed> Breed { get; set; } = null!;
    public DbSet<Employee> Employee { get; set; } = null!;
    public DbSet<EmployeeAddForm> EmployeeAddForm { get; set; } = null!;
    public DbSet<JobType> JobType { get; set; } = null!;
    public DbSet<Origin> Origin { get; set; } = null!;
    public DbSet<Size> Size { get; set; } = null!;
    public DbSet<Status> Status { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;
    public DbSet<UserRole> UserRole { get; set; } = null!;
    public DbSet<Visit> Visit { get; set; } = null!;
    public DbSet<VisitStatus> VisitStatus { get; set; } = null!;
    public DbSet<WorkShift> WorkShift { get; set; } = null!;
}