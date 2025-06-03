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
    /// Obtient la liste des muscles avec pagination
    /// </summary>
    /// <param name="paginationParams">Paramètres de pagination</param>
    /// <param name="groupeMusculaireId">ID du groupe musculaire pour filtrer (optionnel)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MuscleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MuscleDto>>> GetMuscles(
        [FromQuery] PaginationParams paginationParams,
        [FromQuery] int? groupeMusculaireId = null)
    {
        var result = await _muscleService.GetMusclesAsync(paginationParams, groupeMusculaireId);
        return Ok(result);
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