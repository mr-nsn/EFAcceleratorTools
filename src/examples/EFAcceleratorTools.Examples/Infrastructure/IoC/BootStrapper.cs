using EFAcceleratorTools.Examples.Configs;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;
using EFAcceleratorTools.Examples.Logging;
using EFAcceleratorTools.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EFAcceleratorTools.Examples.Infrastructure.IoC;

public static class BootStrapper
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfig = configuration.GetSection("ConsoleAppConfiguration");
        var config = appConfig.Get<ConsoleAppConfiguration>();

        #region Configuration

        services.Configure<ConsoleAppConfiguration>(appConfig);
        services.AddTransient<Program>();

        #endregion

        #region Context

        services.AddDbContext<DataContext>(options => {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(config?.ConnectionConfiguration.ConnectionStrings.DefaultConnection, options =>
            {
                var timeout = config?.ConnectionConfiguration.MaxTimeoutInSeconds;
                options.EnableRetryOnFailure();
                options.CommandTimeout(timeout);
            });
        }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);

        services.AddDbContextFactory<DataContext, DataContextFactory>(lifetime: ServiceLifetime.Scoped);

        #endregion

        #region Repositories

        services.AddScoped<ICourseRepository, CourseRepository>();

        #endregion

        #region Logging

        services.AddScoped<IApplicationLogger, ApplicationLogger>();

        #endregion
    }
}
