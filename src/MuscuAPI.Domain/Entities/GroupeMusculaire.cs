namespace MuscuAPI.Domain.Entities;

public class GroupeMusculaire
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Ordre { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Muscle> Muscles { get; set; } = new List<Muscle>();
}