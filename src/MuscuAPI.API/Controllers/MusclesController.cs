using Microsoft.AspNetCore.Mvc;
using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Application.Services.Interfaces;

namespace MuscuAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusclesController : ControllerBase
{
    private readonly IMuscleService _muscleService;

    public MusclesController(IMuscleService muscleService)
    {
        _muscleService = muscleService;
    }

    /// <summary>
    /// Obtient la liste des muscles avec filtres et pagination
    /// </summary>
    /// <param name="filterParams">Paramètres de filtrage et pagination</param>
    /// <remarks>
    /// Exemples d'utilisation :
    /// - /api/muscles?pageNumber=1&amp;pageSize=10
    /// - /api/muscles?groupe=pectoraux
    /// - /api/muscles?groupeMusculaireId=1
    /// - /api/muscles?searchTerm=biceps
    /// - /api/muscles?sortBy=recent&amp;includeInactive=true
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MuscleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MuscleDto>>> GetMuscles(
        [FromQuery] MuscleFilterParams filterParams)
    {
        var result = await _muscleService.GetMusclesAsync(filterParams);

        // Ajouter des headers pour la pagination
        Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
        Response.Headers.Add("X-Pagination-PageNumber", result.PageNumber.ToString());
        Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
        Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());

        return Ok(result);
    }

    /// <summary>
    /// Recherche des muscles par nom ou nom latin
    /// </summary>
    /// <param name="q">Terme de recherche</param>
    /// <remarks>
    /// Recherche dans le nom et le nom latin des muscles.
    /// Exemple : /api/muscles/search?q=biceps
    /// </remarks>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<MuscleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MuscleDto>>> SearchMuscles([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Le terme de recherche ne peut pas être vide." });
        }

        var muscles = await _muscleService.SearchMusclesAsync(q);
        return Ok(muscles);
    }

    /// <summary>
    /// Obtient un muscle par son ID
    /// </summary>
    /// <param name="id">ID du muscle</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MuscleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MuscleDto>> GetMuscle(int id)
    {
        var muscle = await _muscleService.GetMuscleByIdAsync(id);
        if (muscle == null)
        {
            return NotFound(new { message = $"Muscle avec l'ID {id} non trouvé." });
        }
        return Ok(muscle);
    }

    /// <summary>
    /// Crée un nouveau muscle
    /// </summary>
    /// <param name="createMuscleDto">Données du muscle à créer</param>
    [HttpPost]
    [ProducesResponseType(typeof(MuscleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MuscleDto>> CreateMuscle(CreateMuscleDto createMuscleDto)
    {
        try
        {
            var muscle = await _muscleService.CreateMuscleAsync(createMuscleDto);
            return CreatedAtAction(nameof(GetMuscle), new { id = muscle.Id }, muscle);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Met à jour un muscle existant
    /// </summary>
    /// <param name="id">ID du muscle</param>
    /// <param name="updateMuscleDto">Nouvelles données du muscle</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MuscleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MuscleDto>> UpdateMuscle(int id, UpdateMuscleDto updateMuscleDto)
    {
        try
        {
            var muscle = await _muscleService.UpdateMuscleAsync(id, updateMuscleDto);
            if (muscle == null)
            {
                return NotFound(new { message = $"Muscle avec l'ID {id} non trouvé." });
            }
            return Ok(muscle);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Supprime un muscle
    /// </summary>
    /// <param name="id">ID du muscle à supprimer</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMuscle(int id)
    {
        var deleted = await _muscleService.DeleteMuscleAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Muscle avec l'ID {id} non trouvé." });
        }
        return NoContent();
    }
}