using MuscuAPI.Domain.Common;

namespace MuscuAPI.Domain.Entities;

public class TypeEquipement : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int Ordre { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
}