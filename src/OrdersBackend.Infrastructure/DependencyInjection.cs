using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Infrastructure.Authentication;
using OrdersBackend.Infrastructure.Persistence;
using OrdersBackend.Infrastructure.Persistence.Repositories;

namespace OrdersBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrdersDb")
            ?? throw new InvalidOperationException("Connection string 'OrdersDb' was not found.");

        services.AddDbContext<OrdersDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IUserAuthenticator, InMemoryUserAuthenticator>();

        return services;
    }
}
