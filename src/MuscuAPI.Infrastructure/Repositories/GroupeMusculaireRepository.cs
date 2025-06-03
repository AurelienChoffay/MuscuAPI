using Microsoft.EntityFrameworkCore;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Domain.Interfaces;
using MuscuAPI.Infrastructure.Data;

namespace MuscuAPI.Infrastructure.Repositories;

public class GroupeMusculaireRepository : Repository<GroupeMusculaire>, IGroupeMusculaireRepository
{
    public GroupeMusculaireRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GroupeMusculaire>> GetActiveGroupesAsync()
    {
        return await _dbSet
            .Where(g => g.IsActive)
            .OrderBy(g => g.Ordre)
            .ToListAsync();
    }

    public async Task<GroupeMusculaire?> GetByIdWithMusclesAsync(int id)
    {
        return await _dbSet
            .Include(g => g.Muscles)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> HasMusclesAsync(int id)
    {
        return await _context.Muscles
            .AnyAsync(m => m.GroupeMusculaireId == id);
    }
}