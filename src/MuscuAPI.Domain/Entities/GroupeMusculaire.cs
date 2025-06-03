namespace MuscuAPI.Domain.Entities;

public class GroupeMusculaire
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Ordre { get; set; } // Pour trier l'affichage
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Muscle> Muscles { get; set; } = new List<Muscle>();
}