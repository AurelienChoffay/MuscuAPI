using MuscuAPI.Domain.Common;

namespace MuscuAPI.Domain.Entities;

public class ExerciceImage : BaseEntity
{
    public int ExerciceId { get; set; }
    public virtual Exercice Exercice { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Ordre { get; set; } // Pour ordonner les images
    public bool IsPrincipal { get; set; } // Image principale
}