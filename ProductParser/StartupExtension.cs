using Microsoft.EntityFrameworkCore;
using ProductParser.DAL;

namespace ProductParser;

public static class StartupExtension
{
    public static void AddService(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("DefaultConnection");
        builder.Services.AddDbContextPool<IntegrationDbContext>
            (o => o.UseNpgsql(connectionString));
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