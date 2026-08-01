using System.Reflection;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Infrastructure.Data;
using TechAssessment.Infrastructure.Data.Interceptors;
using TechAssessment.Infrastructure.Identity;
using TechAssessment.Infrastructure.Messaging;
using TechAssessment.Infrastructure.Notifications;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");
        //Why this package Aspire.Microsoft.EntityFrameworkCore.SqlServer used right now i don't installed that package but i can use the sql container and its connection string which reference from Aspire Host application?
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString);
            options.UseSqlServer(connectionString, sqloptions => {
                sqloptions.EnableRetryOnFailure( 
                    maxRetryCount: 5, 
                    maxRetryDelay: TimeSpan.FromSeconds(30), 
                    errorNumbersToAdd: null);
            });
            
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        //builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddSingleton<ServiceBusClient>(_ =>
        {
            var serviceBusConnectionString = builder.Configuration.GetConnectionString(Services.ServiceBusConnection);
            var options = new ServiceBusClientOptions
            {
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential, // Delays grow with each failure
                    MaxRetries = 3,                         // Stop trying network after 3 attempts
                    Delay = TimeSpan.FromSeconds(0.8),      // Base delay
                    MaxDelay = TimeSpan.FromSeconds(60),    // Cap the maximum delay
                    TryTimeout = TimeSpan.FromSeconds(60)   // Timeout per network call
                }
            };
            return new ServiceBusClient(serviceBusConnectionString, options);
        });

        builder.Services.AddScoped<IServiceBusPublisher, AzureServiceBusPublisher>();
        builder.Services.AddScoped<IPublisher, Publisher>();


        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies()
            ;

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }

}
