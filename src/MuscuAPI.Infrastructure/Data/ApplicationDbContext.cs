using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MuscuAPI.Domain.Entities;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MuscuAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<GroupeMusculaire> GroupesMusculaires => Set<GroupeMusculaire>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

// Créer une migration initiale pour vérifier la configuration
// dotnet ef migrations add InitialCreate -p src/MuscuAPI.Infrastructure -s src/MuscuAPI.API

// Créer la base de données
// dotnet ef database update -p src/MuscuAPI.Infrastructure -s src/MuscuAPI.API