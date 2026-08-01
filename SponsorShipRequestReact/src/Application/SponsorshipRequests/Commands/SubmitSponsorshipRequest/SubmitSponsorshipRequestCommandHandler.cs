
using TechAssessment.Application.Common.Exceptions;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public class SubmitSponsorshipRequestCommandHandler : IRequestHandler<SubmitSponsorshipRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public SubmitSponsorshipRequestCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }

    public async Task<int> Handle(SubmitSponsorshipRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SponsorshipRequests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (entity.Status != SponsorshipRequestStatus.Draft)
        {
            throw new InvalidOperationException("Only draft requests can be submitted.");
        }

        if (entity.RequestorId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        entity.Status = SponsorshipRequestStatus.PendingManagerApproval;
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
