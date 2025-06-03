using FluentAssertions;
using Moq;
using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Application.Services;
using MuscuAPI.Domain.Entities;
using MuscuAPI.Domain.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace MuscuAPI.Application.Tests.Services;

public class MuscleServiceTests
{
    private readonly Mock<IMuscleRepository> _muscleRepositoryMock;
    private readonly Mock<IGroupeMusculaireRepository> _groupeRepositoryMock;
    private readonly MuscleService _service;

    public MuscleServiceTests()
    {
        _muscleRepositoryMock = new Mock<IMuscleRepository>();
        _groupeRepositoryMock = new Mock<IGroupeMusculaireRepository>();
        _service = new MuscleService(_muscleRepositoryMock.Object, _groupeRepositoryMock.Object);
    }

    [Fact]
    public async Task GetMusclesAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var muscles = new List<Muscle>
        {
            new Muscle
            {
                Id = 1,
                Nom = "Grand pectoral",
                GroupeMusculaireId = 1,
                GroupeMusculaire = new GroupeMusculaire { Id = 1, Nom = "Pectoraux" }
            },
            new Muscle
            {
                Id = 2,
                Nom = "Petit pectoral",
                GroupeMusculaireId = 1,
                GroupeMusculaire = new GroupeMusculaire { Id = 1, Nom = "Pectoraux" }
            }
        };

        _muscleRepositoryMock
            .Setup(r => r.GetFilteredMusclesAsync(
                It.IsAny<Expression<Func<Muscle, bool>>>(),
                It.IsAny<Func<IQueryable<Muscle>, IOrderedQueryable<Muscle>>>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<bool>()))
            .ReturnsAsync((muscles, 2));

        var filterParams = new MuscleFilterParams { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetMusclesAsync(filterParams);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetMuscleByIdAsync_ShouldReturnMuscle_WhenExists()
    {
        // Arrange
        var muscle = new Muscle
        {
            Id = 1,
            Nom = "Grand pectoral",
            GroupeMusculaire = new GroupeMusculaire { Id = 1, Nom = "Pectoraux" }
        };

        _muscleRepositoryMock
            .Setup(r => r.GetByIdWithGroupeAsync(1))
            .ReturnsAsync(muscle);

        // Act
        var result = await _service.GetMuscleByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Nom.Should().Be("Grand pectoral");
        result.GroupeMusculaireNom.Should().Be("Pectoraux");
    }

    [Fact]
    public async Task GetMuscleByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _muscleRepositoryMock
            .Setup(r => r.GetByIdWithGroupeAsync(999))
            .ReturnsAsync((Muscle?)null);

        // Act
        var result = await _service.GetMuscleByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateMuscleAsync_ShouldCreateMuscle_WhenValid()
    {
        // Arrange
        var createDto = new CreateMuscleDto
        {
            Nom = "Deltoïde",
            GroupeMusculaireId = 1
        };

        _muscleRepositoryMock
            .Setup(r => r.ExistsByNomAsync(createDto.Nom, null))
            .ReturnsAsync(false);

        _groupeRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<GroupeMusculaire, bool>>>()))
            .ReturnsAsync(true);

        _muscleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Muscle>()))
            .ReturnsAsync((Muscle m) => m);

        _muscleRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _muscleRepositoryMock
            .Setup(r => r.GetByIdWithGroupeAsync(It.IsAny<int>()))
            .ReturnsAsync(new Muscle
            {
                Id = 1,
                Nom = "Deltoïde",
                GroupeMusculaire = new GroupeMusculaire { Id = 1, Nom = "Épaules" }
            });

        // Act
        var result = await _service.CreateMuscleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Nom.Should().Be("Deltoïde");
        _muscleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Muscle>()), Times.Once);
        _muscleRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateMuscleAsync_ShouldThrow_WhenNomExists()
    {
        // Arrange
        var createDto = new CreateMuscleDto
        {
            Nom = "Grand pectoral",
            GroupeMusculaireId = 1
        };

        _muscleRepositoryMock
            .Setup(r => r.ExistsByNomAsync(createDto.Nom, null))
            .ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateMuscleAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*existe déjà*");
    }

    [Fact]
    public async Task CreateMuscleAsync_ShouldThrow_WhenGroupeNotExists()
    {
        // Arrange
        var createDto = new CreateMuscleDto
        {
            Nom = "Deltoïde",
            GroupeMusculaireId = 999
        };

        _muscleRepositoryMock
            .Setup(r => r.ExistsByNomAsync(createDto.Nom, null))
            .ReturnsAsync(false);

        _groupeRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<GroupeMusculaire, bool>>>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _service.Invoking(s => s.CreateMuscleAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*n'existe pas*");
    }

    [Fact]
    public async Task UpdateMuscleAsync_ShouldUpdateMuscle_WhenValid()
    {
        // Arrange
        var existingMuscle = new Muscle { Id = 1, Nom = "Grand pectoral", GroupeMusculaireId = 1 };
        var updateDto = new UpdateMuscleDto
        {
            Nom = "Grand pectoral modifié",
            GroupeMusculaireId = 1
        };

        _muscleRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingMuscle);

        _muscleRepositoryMock
            .Setup(r => r.ExistsByNomAsync(updateDto.Nom, 1))
            .ReturnsAsync(false);

        _groupeRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<GroupeMusculaire, bool>>>()))
            .ReturnsAsync(true);

        _muscleRepositoryMock
            .Setup(r => r.GetByIdWithGroupeAsync(1))
            .ReturnsAsync(new Muscle
            {
                Id = 1,
                Nom = "Grand pectoral modifié",
                GroupeMusculaire = new GroupeMusculaire { Id = 1, Nom = "Pectoraux" }
            });

        // Act
        var result = await _service.UpdateMuscleAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Nom.Should().Be("Grand pectoral modifié");
        _muscleRepositoryMock.Verify(r => r.Update(It.IsAny<Muscle>()), Times.Once);
        _muscleRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteMuscleAsync_ShouldReturnTrue_WhenDeleted()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Nom = "Grand pectoral" };

        _muscleRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(muscle);

        // Act
        var result = await _service.DeleteMuscleAsync(1);

        // Assert
        result.Should().BeTrue();
        _muscleRepositoryMock.Verify(r => r.Remove(muscle), Times.Once);
        _muscleRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteMuscleAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        _muscleRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Muscle?)null);

        // Act
        var result = await _service.DeleteMuscleAsync(999);

        // Assert
        result.Should().BeFalse();
        _muscleRepositoryMock.Verify(r => r.Remove(It.IsAny<Muscle>()), Times.Never);
    }

    [Fact]
    public async Task SearchMusclesAsync_ShouldReturnMuscles_WhenTermProvided()
    {
        // Arrange
        var muscles = new List<Muscle>
        {
            new Muscle
            {
                Id = 1,
                Nom = "Biceps brachial",
                GroupeMusculaire = new GroupeMusculaire { Id = 4, Nom = "Biceps" }
            }
        };

        _muscleRepositoryMock
            .Setup(r => r.SearchByNomAsync("biceps"))
            .ReturnsAsync(muscles);

        // Act
        var result = await _service.SearchMusclesAsync("biceps");

        // Assert
        result.Should().HaveCount(1);
        result.First().Nom.Should().Be("Biceps brachial");
    }

    [Fact]
    public async Task SearchMusclesAsync_ShouldReturnEmpty_WhenTermEmpty()
    {
        // Act
        var result = await _service.SearchMusclesAsync("");

        // Assert
        result.Should().BeEmpty();
        _muscleRepositoryMock.Verify(r => r.SearchByNomAsync(It.IsAny<string>()), Times.Never);
    }
}