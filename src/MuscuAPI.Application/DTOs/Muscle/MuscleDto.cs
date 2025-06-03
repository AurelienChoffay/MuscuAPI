namespace MuscuAPI.Application.DTOs.Muscle;

public class MuscleDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? NomLatin { get; set; }
    public int GroupeMusculaireId { get; set; }
    public string GroupeMusculaireNom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Fonction { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}