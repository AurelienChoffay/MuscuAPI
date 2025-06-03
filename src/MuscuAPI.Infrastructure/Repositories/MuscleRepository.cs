using Microsoft.EntityFrameworkCore;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Domain.Interfaces;
using MuscuAPI.Infrastructure.Data;
using System.Linq.Expressions;
using System.Linq;

namespace MuscuAPI.Infrastructure.Repositories;

public class MuscleRepository : Repository<Muscle>, IMuscleRepository
{
    public MuscleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Muscle>> GetByGroupeMusculaireAsync(int groupeMusculaireId)
    {
        return await _dbSet
            .Include(m => m.GroupeMusculaire)
            .Where(m => m.GroupeMusculaireId == groupeMusculaireId && m.IsActive)
            .OrderBy(m => m.Nom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Muscle>> GetActiveMusclesAsync()
    {
        return await _dbSet
            .Include(m => m.GroupeMusculaire)
            .Where(m => m.IsActive)
            .OrderBy(m => m.GroupeMusculaire.Ordre)
            .ThenBy(m => m.Nom)
            .ToListAsync();
    }

    public async Task<Muscle?> GetByIdWithGroupeAsync(int id)
    {
        return await _dbSet
            .Include(m => m.GroupeMusculaire)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Muscle>> GetAllWithGroupesAsync()
    {
        return await _dbSet
            .Include(m => m.GroupeMusculaire)
            .OrderBy(m => m.GroupeMusculaire.Ordre)
            .ThenBy(m => m.Nom)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNomAsync(string nom, int? excludeId = null)
    {
        var query = _dbSet.Where(m => m.Nom.ToLower() == nom.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Muscle>> SearchByNomAsync(string searchTerm)
    {
        return await _dbSet
            .Include(m => m.GroupeMusculaire)
            .Where(m => m.Nom.Contains(searchTerm) ||
                       (m.NomLatin != null && m.NomLatin.Contains(searchTerm)))
            .OrderBy(m => m.Nom)
            .ToListAsync();
    }

    // Override pour ajouter automatiquement les timestamps
    public override async Task<Muscle> AddAsync(Muscle entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        return await base.AddAsync(entity);
    }

    public override void Update(Muscle entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        base.Update(entity);
    }

    public async Task<(IEnumerable<Muscle> muscles, int totalCount)> GetFilteredMusclesAsync(
    Expression<Func<Muscle, bool>>? filter = null,
    Func<IQueryable<Muscle>, IOrderedQueryable<Muscle>>? orderBy = null,
    int? skip = null,
    int? take = null,
    bool includeGroupe = true)
    {
        IQueryable<Muscle> query = _dbSet;

        if (includeGroupe)
        {
            query = query.Include(m => m.GroupeMusculaire);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Compter avant la pagination
        var totalCount = await query.CountAsync();

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        var muscles = await query.ToListAsync();
        return (muscles, totalCount);
    }

    public async Task<IEnumerable<Muscle>> GetByGroupeNomAsync(string groupeNom, bool activeOnly = true)
    {
        var query = _dbSet
            .Include(m => m.GroupeMusculaire)
            .Where(m => m.GroupeMusculaire.Nom.ToLower() == groupeNom.ToLower());

        if (activeOnly)
        {
            query = query.Where(m => m.IsActive);
        }

        return await query
            .OrderBy(m => m.Nom)
            .ToListAsync();
    }
}