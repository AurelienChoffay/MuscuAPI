using MuscuAPI.Domain.Entities;

namespace MuscuAPI.Domain.Interfaces;

public interface IMuscleRepository : IRepository<Muscle>
{
    // Méthodes spécifiques aux muscles
    Task<IEnumerable<Muscle>> GetByGroupeMusculaireAsync(int groupeMusculaireId);
    Task<IEnumerable<Muscle>> GetActiveMusclesAsync();
    Task<Muscle?> GetByIdWithGroupeAsync(int id);
    Task<IEnumerable<Muscle>> GetAllWithGroupesAsync();
    Task<bool> ExistsByNomAsync(string nom, int? excludeId = null);
    Task<IEnumerable<Muscle>> SearchByNomAsync(string searchTerm);
}