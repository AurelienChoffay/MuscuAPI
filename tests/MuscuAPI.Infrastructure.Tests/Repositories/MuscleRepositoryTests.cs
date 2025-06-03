using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Infrastructure.Data;
using MuscuAPI.Infrastructure.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace MuscuAPI.Infrastructure.Tests.Repositories;

public class MuscleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MuscleRepository _repository;

    public MuscleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MuscleRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        var groupePectoraux = new GroupeMusculaire { Id = 1, Nom = "Pectoraux", Ordre = 1 };
        var groupeDos = new GroupeMusculaire { Id = 2, Nom = "Dos", Ordre = 2 };

        _context.GroupesMusculaires.AddRange(groupePectoraux, groupeDos);

        var muscles = new[]
        {
            new Muscle
            {
                Id = 1,
                Nom = "Grand pectoral",
                NomLatin = "Pectoralis major",
                GroupeMusculaireId = 1,
                GroupeMusculaire = groupePectoraux,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Muscle
            {
                Id = 2,
                Nom = "Petit pectoral",
                NomLatin = "Pectoralis minor",
                GroupeMusculaireId = 1,
                GroupeMusculaire = groupePectoraux,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Muscle
            {
                Id = 3,
                Nom = "Grand dorsal",
                NomLatin = "Latissimus dorsi",
                GroupeMusculaireId = 2,
                GroupeMusculaire = groupeDos,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Muscle
            {
                Id = 4,
                Nom = "Trapèze",
                GroupeMusculaireId = 2,
                GroupeMusculaire = groupeDos,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Muscles.AddRange(muscles);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMuscle_WhenExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nom.Should().Be("Grand pectoral");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveMusclesAsync_ShouldReturnOnlyActiveMuscles()
    {
        // Act
        var result = await _repository.GetActiveMusclesAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(m => m.IsActive);
        result.First().GroupeMusculaire.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByGroupeMusculaireAsync_ShouldReturnMusclesFromGroup()
    {
        // Act
        var result = await _repository.GetByGroupeMusculaireAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m => m.GroupeMusculaireId == 1);
        result.Should().OnlyContain(m => m.IsActive);
    }

    [Fact]
    public async Task SearchByNomAsync_ShouldFindByNom()
    {
        // Act
        var result = await _repository.SearchByNomAsync("pectoral");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m => m.Nom.Contains("pectoral", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchByNomAsync_ShouldFindByNomLatin()
    {
        // Act
        var result = await _repository.SearchByNomAsync("dorsi");

        // Assert
        result.Should().HaveCount(1);
        result.First().NomLatin.Should().Contain("dorsi");
    }

    [Fact]
    public async Task ExistsByNomAsync_ShouldReturnTrue_WhenExists()
    {
        // Act
        var result = await _repository.ExistsByNomAsync("Grand pectoral");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNomAsync_ShouldReturnFalse_WhenExcludingItself()
    {
        // Act
        var result = await _repository.ExistsByNomAsync("Grand pectoral", excludeId: 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFilteredMusclesAsync_ShouldApplyFiltersCorrectly()
    {
        // Arrange
        Expression<Func<Muscle, bool>> filter = m => m.IsActive && m.GroupeMusculaireId == 1;
        Func<IQueryable<Muscle>, IOrderedQueryable<Muscle>> orderBy = q => q.OrderBy(m => m.Nom);

        // Act
        var (muscles, totalCount) = await _repository.GetFilteredMusclesAsync(
            filter: filter,
            orderBy: orderBy,
            skip: 0,
            take: 10
        );

        // Assert
        totalCount.Should().Be(2);
        muscles.Should().HaveCount(2);
        muscles.First().Nom.Should().Be("Grand pectoral");
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAt()
    {
        // Arrange
        var newMuscle = new Muscle
        {
            Nom = "Deltoïde",
            GroupeMusculaireId = 1
        };

        // Act
        var result = await _repository.AddAsync(newMuscle);
        await _repository.SaveChangesAsync();

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Update_ShouldSetUpdatedAt()
    {
        // Arrange
        var muscle = await _repository.GetByIdAsync(1);

        // Act
        muscle!.Nom = "Grand pectoral modifié";
        _repository.Update(muscle);
        await _repository.SaveChangesAsync();

        // Assert
        muscle.UpdatedAt.Should().NotBeNull();
        muscle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}