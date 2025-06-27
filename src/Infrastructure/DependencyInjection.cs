using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Constants;
using ProjectTemplate.Infrastructure.Data;
using ProjectTemplate.Infrastructure.Data.Interceptors;

namespace ProjectTemplate.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("ProjectTemplateDb");
        Guard.Against.Null(connectionString, message: "Connection string 'ProjectTemplateDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddAsyncSeeding(sp);
        });
    
        //builder.Services.Add
        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitializer>();


        builder.Services.AddSingleton(TimeProvider.System);






    }
}
