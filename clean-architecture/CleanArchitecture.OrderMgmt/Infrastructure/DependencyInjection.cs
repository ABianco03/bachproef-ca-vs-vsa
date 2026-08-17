using Application.Products;
using FluentValidation;
using Infrastructure.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();


        return services;
    }
}