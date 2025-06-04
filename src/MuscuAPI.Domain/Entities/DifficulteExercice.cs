using MuscuAPI.Domain.Common;

namespace MuscuAPI.Domain.Entities;

public class DifficulteExercice : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Niveau { get; set; } // 1=Débutant, 2=Intermédiaire, 3=Avancé, 4=Expert
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
}