using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MuscuAPI.API.Controllers;
using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Application.Services.Interfaces;
using Xunit;

namespace MuscuAPI.API.Tests.Controllers;

public class MusclesControllerTests
{
    private readonly Mock<IMuscleService> _muscleServiceMock;
    private readonly MusclesController _controller;

    public MusclesControllerTests()
    {
        _muscleServiceMock = new Mock<IMuscleService>();
        _controller = new MusclesController(_muscleServiceMock.Object);
    }

    [Fact]
    public async Task GetMuscles_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<MuscleDto>
        {
            Items = new List<MuscleDto>
            {
                new MuscleDto { Id = 1, Nom = "Grand pectoral" }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _muscleServiceMock
            .Setup(s => s.GetMusclesAsync(It.IsAny<MuscleFilterParams>()))
            .ReturnsAsync(pagedResult);

        var filterParams = new MuscleFilterParams();

        // Act
        var result = await _controller.GetMuscles(filterParams);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(pagedResult);

        _controller.Response.Headers.Should().ContainKey("X-Pagination-TotalCount");
    }

    [Fact]
    public async Task GetMuscle_ShouldReturnOk_WhenExists()
    {
        // Arrange
        var muscleDto = new MuscleDto { Id = 1, Nom = "Grand pectoral" };

        _muscleServiceMock
            .Setup(s => s.GetMuscleByIdAsync(1))
            .ReturnsAsync(muscleDto);

        // Act
        var result = await _controller.GetMuscle(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(muscleDto);
    }

    [Fact]
    public async Task GetMuscle_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _muscleServiceMock
            .Setup(s => s.GetMuscleByIdAsync(999))
            .ReturnsAsync((MuscleDto?)null);

        // Act
        var result = await _controller.GetMuscle(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateMuscle_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var createDto = new CreateMuscleDto { Nom = "Deltoïde", GroupeMusculaireId = 1 };
        var muscleDto = new MuscleDto { Id = 1, Nom = "Deltoïde" };

        _muscleServiceMock
            .Setup(s => s.CreateMuscleAsync(createDto))
            .ReturnsAsync(muscleDto);

        // Act
        var result = await _controller.CreateMuscle(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(muscleDto);
        createdResult.ActionName.Should().Be(nameof(MusclesController.GetMuscle));
    }

    [Fact]
    public async Task CreateMuscle_ShouldReturnBadRequest_WhenInvalid()
    {
        // Arrange
        var createDto = new CreateMuscleDto { Nom = "Existing", GroupeMusculaireId = 1 };

        _muscleServiceMock
            .Setup(s => s.CreateMuscleAsync(createDto))
            .ThrowsAsync(new InvalidOperationException("Muscle exists"));

        // Act
        var result = await _controller.CreateMuscle(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateMuscle_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var updateDto = new UpdateMuscleDto { Nom = "Updated", GroupeMusculaireId = 1 };
        var muscleDto = new MuscleDto { Id = 1, Nom = "Updated" };

        _muscleServiceMock
            .Setup(s => s.UpdateMuscleAsync(1, updateDto))
            .ReturnsAsync(muscleDto);

        // Act
        var result = await _controller.UpdateMuscle(1, updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateMuscle_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateMuscleDto { Nom = "Updated", GroupeMusculaireId = 1 };

        _muscleServiceMock
            .Setup(s => s.UpdateMuscleAsync(999, updateDto))
            .ReturnsAsync((MuscleDto?)null);

        // Act
        var result = await _controller.UpdateMuscle(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteMuscle_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        _muscleServiceMock
            .Setup(s => s.DeleteMuscleAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteMuscle(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteMuscle_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _muscleServiceMock
            .Setup(s => s.DeleteMuscleAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteMuscle(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task SearchMuscles_ShouldReturnOk_WhenTermProvided()
    {
        // Arrange
        var muscles = new List<MuscleDto>
        {
            new MuscleDto { Id = 1, Nom = "Biceps" }
        };

        _muscleServiceMock
            .Setup(s => s.SearchMusclesAsync("biceps"))
            .ReturnsAsync(muscles);

        // Act
        var result = await _controller.SearchMuscles("biceps");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(muscles);
    }

    [Fact]
    public async Task SearchMuscles_ShouldReturnBadRequest_WhenTermEmpty()
    {
        // Act
        var result = await _controller.SearchMuscles("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}