using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using School.Infrastructure.Data;

namespace School.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSchoolInfrastructure(this IServiceCollection services)
    {
        services.AddDbContextFactory<SchoolDbContext>(options =>
            options.UseSqlite("Data Source=school.db"));
        services.AddSingleton<ISchoolRepository, SchoolRepository>();
        return services;
    }

    public static void InitializeSchoolDatabase(this IServiceProvider services)
    {
        using var db = services.GetRequiredService<IDbContextFactory<SchoolDbContext>>().CreateDbContext();
        db.Database.EnsureCreated();
        SchoolDbSeeder.Seed(db);
    }
}
