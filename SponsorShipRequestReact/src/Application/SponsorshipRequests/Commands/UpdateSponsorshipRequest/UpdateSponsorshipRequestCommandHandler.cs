using Ardalis.GuardClauses;
using TechAssessment.Application.Common.Exceptions;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public class UpdateSponsorshipRequestCommandHandler : IRequestHandler<UpdateSponsorshipRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateSponsorshipRequestCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(UpdateSponsorshipRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SponsorshipRequests
            .FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // Only requestor can update draft requests
        if (entity.RequestorId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        // Only draft requests can be updated
        if (entity.Status != SponsorshipRequestStatus.Draft)
        {
            throw new InvalidOperationException("Only draft requests can be updated.");
        }

        entity.Title = request.Title;
        entity.Department = request.Department;
        entity.SponsorshipTypeId = request.SponsorshipTypeId;
        entity.EventName = request.EventName;
        entity.EventDate = request.EventDate;
        entity.RequestedAmount = request.RequestedAmount;
        entity.Purpose = request.Purpose;
        entity.BusinessBenefit = request.BusinessBenefit;
        entity.SupportingDocumentUrl = request.SupportingDocumentUrl;
        entity.LastModified = DateTime.UtcNow;

        _context.SponsorshipRequests.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
