using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechAssessment.Application.Common.Behaviours;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application;

public static class RequestHandlerRegistrationExtensions
{
    public static IServiceCollection AddDecoratedRequestHandler<TRequest, TResponse, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler: class, IRequestHandler<TRequest, TResponse>
    {
        services.AddTransient<THandler>();

        services.AddTransient<IRequestHandler<TRequest, TResponse>>(provider =>
        {
            IRequestHandler<TRequest, TResponse> handler = provider.GetRequiredService<THandler>();

            handler = new ValidationBehaviour<TRequest, TResponse>(
                handler,
                 provider.GetServices<IValidator<TRequest>>()
                 );

            handler = new LoggingBehaviour<TRequest, TResponse>(
                 provider.GetRequiredService<ILogger<TRequest>>(),
                provider.GetService<IUser>()!,
                provider.GetService<IIdentityService>()!,
                handler
                 );

            return handler;
        });

        return services;
    }


}
