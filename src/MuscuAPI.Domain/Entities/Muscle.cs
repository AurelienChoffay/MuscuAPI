namespace MuscuAPI.Domain.Entities;

public class Muscle
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? NomLatin { get; set; }

    // Relation avec GroupeMusculaire
    public int GroupeMusculaireId { get; set; }
    public GroupeMusculaire GroupeMusculaire { get; set; } = null!;

    public string? Description { get; set; }
    public string? Fonction { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<Exercice> ExercicesPrincipal { get; set; } = new List<Exercice>();
    public virtual ICollection<ExerciceMuscleSecondaire> ExercicesSecondaire { get; set; } = new List<ExerciceMuscleSecondaire>();
}