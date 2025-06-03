using Microsoft.EntityFrameworkCore;
using MuscuAPI.Application.Services;
using MuscuAPI.Application.Services.Interfaces;
using MuscuAPI.Domain.Interfaces;
using MuscuAPI.Infrastructure.Data;
using MuscuAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MuscuAPI", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMuscleRepository, MuscleRepository>();
builder.Services.AddScoped<IGroupeMusculaireRepository, GroupeMusculaireRepository>();

// Register services
builder.Services.AddScoped<IMuscleService, MuscleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();