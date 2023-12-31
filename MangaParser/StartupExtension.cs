using Microsoft.EntityFrameworkCore;
using ProductParser.DAL;
using ProductParser.DAL.Repository;
using ProductParser.Service;
using ProductParser.Service.Impl;

namespace ProductParser;

public static class StartupExtension
{
    public static void AddService(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("PostgreSql");
        builder.Services.AddDbContextPool<IntegrationDbContext>
            (o => o.UseNpgsql(connectionString));

        builder.Services.AddScoped<IMangaService, MangaService>();
        builder.Services.AddScoped<IMangaRepository, MangaRepository>();
    }
    public static void AddMigration(this WebApplication app)
    {
        using var serviceScope = app.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        using var context = serviceScope.ServiceProvider.GetService<IntegrationDbContext>();
        context!.Database.Migrate();
    }
}