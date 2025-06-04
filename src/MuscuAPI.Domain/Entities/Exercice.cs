using MuscuAPI.Domain.Common;

namespace MuscuAPI.Domain.Entities;

public class Exercice : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MusclePrincipalId { get; set; }
    public virtual Muscle MusclePrincipal { get; set; } = null!;
    public virtual ICollection<ExerciceMuscleSecondaire> MusclesSecondaires { get; set; } = new List<ExerciceMuscleSecondaire>();
    public int DifficulteId { get; set; }
    public virtual DifficulteExercice Difficulte { get; set; } = null!;

    public int TypeEquipementId { get; set; }
    public virtual TypeEquipement TypeEquipement { get; set; } = null!;
    public string? Instructions { get; set; }
    public string? Conseils { get; set; }
    public string? VideoUrl { get; set; }
    public virtual ICollection<ExerciceImage> Images { get; set; } = new List<ExerciceImage>();
    public bool IsActive { get; set; } = true;
}