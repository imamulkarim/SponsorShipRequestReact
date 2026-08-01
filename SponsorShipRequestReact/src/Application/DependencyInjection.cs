using System.Reflection;
using Microsoft.Extensions.Hosting;
using TechAssessment.Application;
using TechAssessment.Application.Common.Behaviours;
using TechAssessment.Application.SponsorshipRequests.Commands.CreateSponsorshipRequest;
using TechAssessment.Application.SponsorshipRequests.Queries.GetAllRequests;
using TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;
using TechAssessment.Application.SponsorshipRequests.Queries.GetSponsorshipTypes;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg => 
            cfg.AddMaps(Assembly.GetExecutingAssembly()));

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //Manual Service Registration for Mediator
        //builder.Services.AddScoped<ICustomMediator, CustomMediator>();
        //builder.Services.AddDecoratedRequestHandler<GetAllRequestsQuery, AllRequestsVm, GetAllRequestsQueryHandler>();
        //builder.Services.AddDecoratedRequestHandler<GetMyRequestsQuery, MyRequestsVm, GetMyRequestsQueryHandler>();
        //builder.Services.AddDecoratedRequestHandler<GetSponsorshipTypesQuery, SponsorshipTypesVm, GetSponsorshipTypesQueryHandler>();
        //builder.Services.AddDecoratedRequestHandler<CreateSponsorshipRequestCommand, int, CreateSponsorshipRequestCommandHandler>();

        builder.Services.AddCustomMediator(Assembly.GetExecutingAssembly());
        builder.Services.AddPublishers(Assembly.GetExecutingAssembly());


        //builder.Services.AddMediatR(cfg => {
        //    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        //    cfg.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
        //    cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
        //    cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
        //    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        //    cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        //});
    }
}
