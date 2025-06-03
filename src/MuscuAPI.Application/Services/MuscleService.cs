using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Application.Services.Interfaces;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Domain.Interfaces;

namespace MuscuAPI.Application.Services;

public class MuscleService : IMuscleService
{
    private readonly IMuscleRepository _muscleRepository;
    private readonly IGroupeMusculaireRepository _groupeMusculaireRepository;

    public MuscleService(
        IMuscleRepository muscleRepository,
        IGroupeMusculaireRepository groupeMusculaireRepository)
    {
        _muscleRepository = muscleRepository;
        _groupeMusculaireRepository = groupeMusculaireRepository;
    }

    public async Task<PagedResult<MuscleDto>> GetMusclesAsync(
        PaginationParams paginationParams,
        int? groupeMusculaireId = null)
    {
        IEnumerable<Muscle> muscles;

        if (groupeMusculaireId.HasValue)
        {
            muscles = await _muscleRepository.GetByGroupeMusculaireAsync(groupeMusculaireId.Value);
        }
        else
        {
            muscles = await _muscleRepository.GetActiveMusclesAsync();
        }

        var musclesList = muscles.ToList();
        var totalCount = musclesList.Count;

        var pagedMuscles = musclesList
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResult<MuscleDto>
        {
            Items = pagedMuscles,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<MuscleDto?> GetMuscleByIdAsync(int id)
    {
        var muscle = await _muscleRepository.GetByIdWithGroupeAsync(id);
        return muscle == null ? null : MapToDto(muscle);
    }

    public async Task<MuscleDto> CreateMuscleAsync(CreateMuscleDto createMuscleDto)
    {
        // Vérifier que le nom n'existe pas déjà
        if (await _muscleRepository.ExistsByNomAsync(createMuscleDto.Nom))
        {
            throw new InvalidOperationException($"Un muscle avec le nom '{createMuscleDto.Nom}' existe déjà.");
        }

        // Vérifier que le groupe musculaire existe
        var groupeExists = await _groupeMusculaireRepository.ExistsAsync(g => g.Id == createMuscleDto.GroupeMusculaireId);
        if (!groupeExists)
        {
            throw new InvalidOperationException($"Le groupe musculaire avec l'ID {createMuscleDto.GroupeMusculaireId} n'existe pas.");
        }

        var muscle = new Muscle
        {
            Nom = createMuscleDto.Nom,
            NomLatin = createMuscleDto.NomLatin,
            GroupeMusculaireId = createMuscleDto.GroupeMusculaireId,
            Description = createMuscleDto.Description,
            Fonction = createMuscleDto.Fonction,
            ImageUrl = createMuscleDto.ImageUrl,
            IsActive = true
        };

        await _muscleRepository.AddAsync(muscle);
        await _muscleRepository.SaveChangesAsync();

        var createdMuscle = await _muscleRepository.GetByIdWithGroupeAsync(muscle.Id);
        return MapToDto(createdMuscle!);
    }

    public async Task<MuscleDto?> UpdateMuscleAsync(int id, UpdateMuscleDto updateMuscleDto)
    {
        var muscle = await _muscleRepository.GetByIdAsync(id);
        if (muscle == null)
        {
            return null;
        }

        // Vérifier l'unicité du nom (en excluant le muscle actuel)
        if (await _muscleRepository.ExistsByNomAsync(updateMuscleDto.Nom, id))
        {
            throw new InvalidOperationException($"Un autre muscle avec le nom '{updateMuscleDto.Nom}' existe déjà.");
        }

        // Vérifier que le groupe musculaire existe
        var groupeExists = await _groupeMusculaireRepository.ExistsAsync(g => g.Id == updateMuscleDto.GroupeMusculaireId);
        if (!groupeExists)
        {
            throw new InvalidOperationException($"Le groupe musculaire avec l'ID {updateMuscleDto.GroupeMusculaireId} n'existe pas.");
        }

        muscle.Nom = updateMuscleDto.Nom;
        muscle.NomLatin = updateMuscleDto.NomLatin;
        muscle.GroupeMusculaireId = updateMuscleDto.GroupeMusculaireId;
        muscle.Description = updateMuscleDto.Description;
        muscle.Fonction = updateMuscleDto.Fonction;
        muscle.ImageUrl = updateMuscleDto.ImageUrl;
        muscle.IsActive = updateMuscleDto.IsActive;

        _muscleRepository.Update(muscle);
        await _muscleRepository.SaveChangesAsync();

        var updatedMuscle = await _muscleRepository.GetByIdWithGroupeAsync(id);
        return MapToDto(updatedMuscle!);
    }

    public async Task<bool> DeleteMuscleAsync(int id)
    {
        var muscle = await _muscleRepository.GetByIdAsync(id);
        if (muscle == null)
        {
            return false;
        }

        _muscleRepository.Remove(muscle);
        await _muscleRepository.SaveChangesAsync();
        return true;
    }

    private static MuscleDto MapToDto(Muscle muscle)
    {
        return new MuscleDto
        {
            Id = muscle.Id,
            Nom = muscle.Nom,
            NomLatin = muscle.NomLatin,
            GroupeMusculaireId = muscle.GroupeMusculaireId,
            GroupeMusculaireNom = muscle.GroupeMusculaire?.Nom ?? string.Empty,
            Description = muscle.Description,
            Fonction = muscle.Fonction,
            ImageUrl = muscle.ImageUrl,
            IsActive = muscle.IsActive,
            CreatedAt = muscle.CreatedAt,
            UpdatedAt = muscle.UpdatedAt
        };
    }
}