using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MuscuAPI.Domain.Entities;

namespace MuscuAPI.Infrastructure.Data.Configurations;

public class GroupeMusculaireConfiguration : IEntityTypeConfiguration<GroupeMusculaire>
{
    public void Configure(EntityTypeBuilder<GroupeMusculaire> builder)
    {
        builder.ToTable("GroupesMusculaires");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Nom)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.Description)
            .HasMaxLength(200);

        // Données initiales (seed data)
        builder.HasData(
            new GroupeMusculaire { Id = 1, Nom = "Pectoraux", Ordre = 1 },
            new GroupeMusculaire { Id = 2, Nom = "Dos", Ordre = 2 },
            new GroupeMusculaire { Id = 3, Nom = "Épaules", Ordre = 3 },
            new GroupeMusculaire { Id = 4, Nom = "Biceps", Ordre = 4 },
            new GroupeMusculaire { Id = 5, Nom = "Triceps", Ordre = 5 },
            new GroupeMusculaire { Id = 6, Nom = "Avant-bras", Ordre = 6 },
            new GroupeMusculaire { Id = 7, Nom = "Abdominaux", Ordre = 7 },
            new GroupeMusculaire { Id = 8, Nom = "Quadriceps", Ordre = 8 },
            new GroupeMusculaire { Id = 9, Nom = "Ischio-jambiers", Ordre = 9 },
            new GroupeMusculaire { Id = 10, Nom = "Fessiers", Ordre = 10 },
            new GroupeMusculaire { Id = 11, Nom = "Mollets", Ordre = 11 },
            new GroupeMusculaire { Id = 12, Nom = "Trapèzes", Ordre = 12 }
        );
    }
}