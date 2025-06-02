namespace MuscuAPI.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Les services seront ajoutés ici au fur et à mesure
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Les repositories seront ajoutés ici au fur et à mesure
        return services;
    }
}