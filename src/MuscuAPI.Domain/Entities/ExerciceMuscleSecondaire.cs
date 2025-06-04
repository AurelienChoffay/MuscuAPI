namespace MuscuAPI.Domain.Entities;

public class ExerciceMuscleSecondaire
{
    public int ExerciceId { get; set; }
    public virtual Exercice Exercice { get; set; } = null!;

    public int MuscleId { get; set; }
    public virtual Muscle Muscle { get; set; } = null!;

    // Propriétés supplémentaires si nécessaire
    public int? OrdreImportance { get; set; } // 1 = très sollicité, 2 = moyennement, 3 = peu
}