using System.ComponentModel.DataAnnotations;

namespace MuscuAPI.Application.DTOs.Muscle;

public class CreateMuscleDto
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
    public string Nom { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Le nom latin ne doit pas dépasser 100 caractères")]
    public string? NomLatin { get; set; }

    [Required(ErrorMessage = "Le groupe musculaire est obligatoire")]
    [Range(1, int.MaxValue, ErrorMessage = "Le groupe musculaire doit être valide")]
    public int GroupeMusculaireId { get; set; }

    [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères")]
    public string? Description { get; set; }

    [StringLength(500, ErrorMessage = "La fonction ne doit pas dépasser 500 caractères")]
    public string? Fonction { get; set; }

    [Url(ErrorMessage = "L'URL de l'image n'est pas valide")]
    [StringLength(255, ErrorMessage = "L'URL ne doit pas dépasser 255 caractères")]
    public string? ImageUrl { get; set; }
}