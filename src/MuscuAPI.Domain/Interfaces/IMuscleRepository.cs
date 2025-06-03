using MuscuAPI.Domain.Entities;
using System.Linq.Expressions;

namespace MuscuAPI.Domain.Interfaces;

public interface IMuscleRepository : IRepository<Muscle>
{
    // Méthodes existantes
    Task<IEnumerable<Muscle>> GetByGroupeMusculaireAsync(int groupeMusculaireId);
    Task<IEnumerable<Muscle>> GetActiveMusclesAsync();
    Task<Muscle?> GetByIdWithGroupeAsync(int id);
    Task<IEnumerable<Muscle>> GetAllWithGroupesAsync();
    Task<bool> ExistsByNomAsync(string nom, int? excludeId = null);
    Task<IEnumerable<Muscle>> SearchByNomAsync(string searchTerm);
    Task<(IEnumerable<Muscle> muscles, int totalCount)> GetFilteredMusclesAsync(
        Expression<Func<Muscle, bool>>? filter = null,
        Func<IQueryable<Muscle>, IOrderedQueryable<Muscle>>? orderBy = null,
        int? skip = null,
        int? take = null,
        bool includeGroupe = true);

    Task<IEnumerable<Muscle>> GetByGroupeNomAsync(string groupeNom, bool activeOnly = true);
}