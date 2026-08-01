using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechAssessment.Application.Common.Behaviours;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application;

public static class MediatorDependencyInjectionExtensions
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services, Assembly assembly)
    {
        // 1. Register the core mediator engine
        services.AddScoped<ICustomMediator, CustomMediator>();

        // 2. Scan the assembly for types implementing our IRequestHandler interface
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)) &&
                        !t.FullName!.Contains("Behaviours"));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)  );

            foreach (var @interface in interfaces)
            {
                var requestType = @interface.GetGenericArguments()[0];
                var responseType = @interface.GetGenericArguments()[1];

                // Check if any IValidator<TRequest> is registered in the DI pipeline for this request
                // We use a factory registration to determine this dynamically at runtime
                services.AddTransient(@interface, provider =>
                {
                    // Resolve the actual inner business logic handler
                    var innerHandler = ActivatorUtilities.CreateInstance(provider, handlerType);


                    //Loging Behaviour Pipeline
                    var loggerInterfaceType = typeof(ILogger<>).MakeGenericType(requestType);
                    var loggers = provider.GetService(loggerInterfaceType);
                    var user = provider.GetService<IUser>();
                    var identityService = provider.GetService<IIdentityService>();
                    if (loggers is not null && user is not null && identityService is not null)
                    {
                        var LogingPipelineType = typeof(LoggingBehaviour<,>).MakeGenericType(requestType, responseType);
                        innerHandler = ActivatorUtilities.CreateInstance(provider, LogingPipelineType, loggers, user, identityService, innerHandler);
                    }

                    //Unhandled Exception Behaviour Pipeline
                    if (loggers is not null)
                    {
                        var exceptionPipelineType = typeof(UnhandledExceptionBehaviour<,>).MakeGenericType(requestType, responseType);
                        innerHandler = ActivatorUtilities.CreateInstance(provider, exceptionPipelineType, loggers, innerHandler);
                    }


                    //Authorization Behaviour Pipeline
                    if (identityService is not null)
                    {
                        var authorizationPipelineType = typeof(AuthorizationBehaviour<,>).MakeGenericType(requestType, responseType);
                        innerHandler = ActivatorUtilities.CreateInstance(provider, authorizationPipelineType, user!, identityService, innerHandler);
                    }


                    // Dynamically look up standard IValidator<TRequest> implementations
                    var validatorInterfaceType = typeof(FluentValidation.IValidator<>).MakeGenericType(requestType);
                    var validators = provider.GetServices(validatorInterfaceType);
                    // If validators exist for this specific request, wrap the handler in the ValidationBehaviour
                    if (validators.Any())
                    {
                        var pipelineType = typeof(ValidationBehaviour<,>).MakeGenericType(requestType, responseType);
                        // Construct the ValidationBehaviour, passing the concrete handler and the found validators
                        innerHandler = ActivatorUtilities.CreateInstance(provider, pipelineType, innerHandler, validators);
                    }


                    //Performance Behaviour Pipeline
                    if (loggers is not null && user is not null && identityService is not null)
                    {
                        var PerformanceBehavooirPipelineType = typeof(PerformanceBehaviour<,>).MakeGenericType(requestType, responseType);
                        innerHandler = ActivatorUtilities.CreateInstance(provider, PerformanceBehavooirPipelineType, loggers, user, identityService, innerHandler);
                    }

                    return innerHandler;
                });
            }
        }

        return services;
    }



    public static IServiceCollection AddPublishers(this IServiceCollection services, Assembly assembly)
    {

        // 2. Scan the assembly for types implementing our IRequestHandler interface
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

            foreach (var @interface in interfaces)
            {
                var requestType = @interface.GetGenericArguments()[0];
                //var responseType = @interface.GetGenericArguments()[1];
                var responseType = requestType;

                // Check if any IValidator<TRequest> is registered in the DI pipeline for this request
                // We use a factory registration to determine this dynamically at runtime
                services.AddTransient(@interface, provider =>
                {
                    // Resolve the actual inner business logic handler
                    var innerHandler = ActivatorUtilities.CreateInstance(provider, handlerType);
                    return innerHandler;
                });
            }
        }

        return services;
    }

}
