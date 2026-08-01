
using TechAssessment.Application.Common.Exceptions;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.CancelRequest;

public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CancelRequestCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SponsorshipRequests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (entity.RequestorId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        if (entity.Status == SponsorshipRequestStatus.Approved || entity.Status == SponsorshipRequestStatus.Rejected)
        {
            throw new InvalidOperationException("Cannot cancel a request that has been approved or rejected.");
        }

        entity.Status = SponsorshipRequestStatus.Cancelled;
        entity.CancelledReason = request.Reason;
        entity.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
