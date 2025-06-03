using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Application.Services.Interfaces;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Domain.Interfaces;
using System.Linq.Expressions;

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

    public async Task<PagedResult<MuscleDto>> GetMusclesAsync(MuscleFilterParams filterParams)
    {
        // Construire l'expression de filtre
        Expression<Func<Muscle, bool>> filter = m => true;

        // Filtre par actif/inactif
        if (!filterParams.IncludeInactive)
        {
            Expression<Func<Muscle, bool>> activeFilter = m => m.IsActive;
            filter = CombineExpressions(filter, activeFilter);
        }

        // Filtre par groupe (ID ou nom)
        if (filterParams.GroupeMusculaireId.HasValue)
        {
            var groupeId = filterParams.GroupeMusculaireId.Value;
            Expression<Func<Muscle, bool>> groupeFilter = m => m.GroupeMusculaireId == groupeId;
            filter = CombineExpressions(filter, groupeFilter);
        }
        else if (!string.IsNullOrWhiteSpace(filterParams.Groupe))
        {
            var groupeNom = filterParams.Groupe.ToLower();
            Expression<Func<Muscle, bool>> groupeFilter = m => m.GroupeMusculaire.Nom.ToLower() == groupeNom;
            filter = CombineExpressions(filter, groupeFilter);
        }

        // Filtre par terme de recherche
        if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
        {
            var searchTerm = filterParams.SearchTerm.ToLower();
            Expression<Func<Muscle, bool>> searchFilter = m =>
                m.Nom.ToLower().Contains(searchTerm) ||
                (m.NomLatin != null && m.NomLatin.ToLower().Contains(searchTerm));
            filter = CombineExpressions(filter, searchFilter);
        }

        // Définir le tri
        Func<IQueryable<Muscle>, IOrderedQueryable<Muscle>> orderBy = filterParams.SortBy?.ToLower() switch
        {
            "nomdesc" => q => q.OrderByDescending(m => m.Nom),
            "groupe" => q => q.OrderBy(m => m.GroupeMusculaire.Ordre).ThenBy(m => m.Nom),
            "recent" => q => q.OrderByDescending(m => m.CreatedAt),
            _ => q => q.OrderBy(m => m.Nom)
        };

        // Calculer la pagination
        var skip = (filterParams.PageNumber - 1) * filterParams.PageSize;

        // Récupérer les données avec le total
        var (muscles, totalCount) = await _muscleRepository.GetFilteredMusclesAsync(
            filter: filter,
            orderBy: orderBy,
            skip: skip,
            take: filterParams.PageSize,
            includeGroupe: true
        );

        var muscleDtos = muscles.Select(MapToDto).ToList();

        return new PagedResult<MuscleDto>
        {
            Items = muscleDtos,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<MuscleDto>> SearchMusclesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<MuscleDto>();
        }

        var muscles = await _muscleRepository.SearchByNomAsync(searchTerm);
        return muscles.Select(MapToDto).ToList();
    }

    // Méthode helper pour combiner les expressions
    private static Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    // Classe helper pour remplacer les paramètres dans les expressions
    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
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