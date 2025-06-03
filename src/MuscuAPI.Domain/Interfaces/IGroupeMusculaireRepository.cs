using MuscuAPI.Domain.Entities;

namespace MuscuAPI.Domain.Interfaces;

public interface IGroupeMusculaireRepository : IRepository<GroupeMusculaire>
{
    Task<IEnumerable<GroupeMusculaire>> GetActiveGroupesAsync();
    Task<GroupeMusculaire?> GetByIdWithMusclesAsync(int id);
    Task<bool> HasMusclesAsync(int id);
}