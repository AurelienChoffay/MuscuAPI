using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MuscuAPI.Application.DTOs.Common;
using MuscuAPI.Application.DTOs.Muscle;
using MuscuAPI.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MuscuAPI.API.Tests.IntegrationTests;

public class MusclesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MusclesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remplacer la DB par InMemory
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Seed data
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                SeedData(db);
            });
        });

        _client = _factory.CreateClient();
    }

    private void SeedData(ApplicationDbContext context)
    {
        context.GroupesMusculaires.Add(new() { Id = 1, Nom = "Pectoraux", Ordre = 1 });
        context.Muscles.Add(new()
        {
            Id = 1,
            Nom = "Grand pectoral",
            GroupeMusculaireId = 1,
            IsActive = true
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetMuscles_ShouldReturnSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/muscles");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MuscleDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateMuscle_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateMuscleDto
        {
            Nom = "Petit pectoral",
            GroupeMusculaireId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/muscles", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var muscle = await response.Content.ReadFromJsonAsync<MuscleDto>();
        muscle.Should().NotBeNull();
        muscle!.Nom.Should().Be("Petit pectoral");
    }

    [Fact]
    public async Task GetMuscle_ShouldReturnNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.GetAsync("/api/muscles/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}