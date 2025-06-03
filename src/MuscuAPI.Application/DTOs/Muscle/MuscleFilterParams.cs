using MuscuAPI.Application.DTOs.Common;

namespace MuscuAPI.Application.DTOs.Muscle;

public class MuscleFilterParams : PaginationParams
{
    /// <summary>
    /// ID du groupe musculaire pour filtrer
    /// </summary>
    public int? GroupeMusculaireId { get; set; }

    /// <summary>
    /// Nom du groupe musculaire pour filtrer (alternative à l'ID)
    /// </summary>
    public string? Groupe { get; set; }

    /// <summary>
    /// Terme de recherche (nom ou nom latin)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Inclure les muscles inactifs
    /// </summary>
    public bool IncludeInactive { get; set; } = false;

    /// <summary>
    /// Tri : "nom", "nomDesc", "groupe", "recent"
    /// </summary>
    public string? SortBy { get; set; } = "nom";
}