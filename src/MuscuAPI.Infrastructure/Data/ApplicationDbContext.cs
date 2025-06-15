using Microsoft.EntityFrameworkCore;
using MuscuAPI.Domain.Entities;

namespace MuscuAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GroupeMusculaire> GroupesMusculaires { get; set; }
    public DbSet<Muscle> Muscles { get; set; }
    public DbSet<DifficulteExercice> DifficultesExercices { get; set; }
    public DbSet<TypeEquipement> TypesEquipements { get; set; }
    public DbSet<Exercice> Exercices { get; set; }
    public DbSet<ExerciceMuscleSecondaire> ExercicesMusclesSecondaires { get; set; }
    public DbSet<ExerciceImage> ExercicesImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration GroupeMusculaire (existant)
        modelBuilder.Entity<GroupeMusculaire>(entity =>
        {
            entity.ToTable("GroupesMusculaires");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nom).IsUnique();
        });

        // Configuration Muscle (existant)
        modelBuilder.Entity<Muscle>(entity =>
        {
            entity.ToTable("Muscles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NomLatin).HasMaxLength(100);
            entity.HasIndex(e => e.Nom).IsUnique();

            entity.HasOne(m => m.GroupeMusculaire)
                .WithMany(g => g.Muscles)
                .HasForeignKey(m => m.GroupeMusculaireId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration DifficulteExercice
        modelBuilder.Entity<DifficulteExercice>(entity =>
        {
            entity.ToTable("DifficultesExercices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.Nom).IsUnique();
            entity.HasIndex(e => e.Niveau);
        });

        // Configuration TypeEquipement
        modelBuilder.Entity<TypeEquipement>(entity =>
        {
            entity.ToTable("TypesEquipements");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IconUrl).HasMaxLength(255);
            entity.HasIndex(e => e.Nom).IsUnique();
            entity.HasIndex(e => e.Ordre);
        });

        // Configuration Exercice
        modelBuilder.Entity<Exercice>(entity =>
        {
            entity.ToTable("Exercices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Instructions).HasMaxLength(2000);
            entity.Property(e => e.Conseils).HasMaxLength(1000);
            entity.Property(e => e.VideoUrl).HasMaxLength(255);
            entity.HasIndex(e => e.Nom);

            entity.HasOne(e => e.MusclePrincipal)
                .WithMany()
                .HasForeignKey(e => e.MusclePrincipalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Difficulte)
                .WithMany(d => d.Exercices)
                .HasForeignKey(e => e.DifficulteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TypeEquipement)
                .WithMany(t => t.Exercices)
                .HasForeignKey(e => e.TypeEquipementId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration ExerciceMuscleSecondaire (Many-to-Many)
        // Configuration ExerciceMuscleSecondaire (Many-to-Many)
        modelBuilder.Entity<ExerciceMuscleSecondaire>(entity =>
        {
            entity.ToTable("ExercicesMusclesSecondaires");
            entity.HasKey(e => new { e.ExerciceId, e.MuscleId });

            entity.HasOne(e => e.Exercice)
                .WithMany(ex => ex.MusclesSecondaires)
                .HasForeignKey(e => e.ExerciceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Muscle)
                .WithMany(m => m.ExercicesSecondaire)
                .HasForeignKey(e => e.MuscleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.OrdreImportance).IsRequired(false);
        });

        // Configuration ExerciceImage
        modelBuilder.Entity<ExerciceImage>(entity =>
        {
            entity.ToTable("ExercicesImages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => new { e.ExerciceId, e.Ordre });

            entity.HasOne(e => e.Exercice)
                .WithMany(ex => ex.Images)
                .HasForeignKey(e => e.ExerciceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data pour les difficultés
        modelBuilder.Entity<DifficulteExercice>().HasData(
            new DifficulteExercice { Id = 1, Nom = "Débutant", Niveau = 1, Description = "Pour les personnes qui commencent la musculation" },
            new DifficulteExercice { Id = 2, Nom = "Intermédiaire", Niveau = 2, Description = "Nécessite quelques mois de pratique" },
            new DifficulteExercice { Id = 3, Nom = "Avancé", Niveau = 3, Description = "Pour les pratiquants expérimentés" },
            new DifficulteExercice { Id = 4, Nom = "Expert", Niveau = 4, Description = "Mouvements complexes nécessitant une excellente maîtrise" }
        );

        // Seed data pour les types d'équipement
        modelBuilder.Entity<TypeEquipement>().HasData(
            new TypeEquipement { Id = 1, Nom = "Aucun équipement", Ordre = 1, Description = "Exercices au poids du corps" },
            new TypeEquipement { Id = 2, Nom = "Haltères", Ordre = 2, Description = "Poids libres modulables" },
            new TypeEquipement { Id = 3, Nom = "Barre", Ordre = 3, Description = "Barre olympique ou EZ" },
            new TypeEquipement { Id = 4, Nom = "Machine", Ordre = 4, Description = "Machines guidées" },
            new TypeEquipement { Id = 5, Nom = "Poulie", Ordre = 5, Description = "Câbles et poulies" },
            new TypeEquipement { Id = 6, Nom = "Élastique/Bande", Ordre = 6, Description = "Bandes de résistance" },
            new TypeEquipement { Id = 7, Nom = "Kettlebell", Ordre = 7, Description = "Poids avec poignée" },
            new TypeEquipement { Id = 8, Nom = "Barre de traction", Ordre = 8, Description = "Pour tractions et suspensions" },
            new TypeEquipement { Id = 9, Nom = "Banc", Ordre = 9, Description = "Banc de musculation" },
            new TypeEquipement { Id = 10, Nom = "Swiss Ball", Ordre = 10, Description = "Ballon de gym" },
            new TypeEquipement { Id = 11, Nom = "TRX", Ordre = 11, Description = "Sangles de suspension" }
        );

        base.OnModelCreating(modelBuilder);
    }
}