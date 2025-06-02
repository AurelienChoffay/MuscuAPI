using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream
using Microsoft.OpenApi.Models;
using MuscuAPI.API.Extensions;
using MuscuAPI.Application.Interfaces;
=======
>>>>>>> Stashed changes
using MuscuAPI.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

<<<<<<< Updated upstream
// Configuration Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les services de l'application
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Add Swagger avec personnalisation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MuscuAPI",
        Version = "v1",
        Description = "API REST pour la gestion d'exercices de musculation",
        Contact = new OpenApiContact
        {
            Name = "Votre Nom",
            Email = "votre.email@example.com"
        }
    });
});
=======
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Swagger - IMPORTANT : Ces lignes sont nécessaires
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
>>>>>>> Stashed changes

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Activer Swagger UI en développement
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Commenter cette ligne pour éviter le warning HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();