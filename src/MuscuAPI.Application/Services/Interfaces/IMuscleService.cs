using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;

namespace MuscuAPI.Application.Services.Interfaces;

public interface IMuscleService
{
    Task<PagedResult<MuscleDto>> GetMusclesAsync(PaginationParams paginationParams, int? groupeMusculaireId = null);
    Task<MuscleDto?> GetMuscleByIdAsync(int id);
    Task<MuscleDto> CreateMuscleAsync(CreateMuscleDto createMuscleDto);
    Task<MuscleDto?> UpdateMuscleAsync(int id, UpdateMuscleDto updateMuscleDto);
    Task<bool> DeleteMuscleAsync(int id);
}