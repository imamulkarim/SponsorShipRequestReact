using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Entities;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.CreateSponsorshipRequest;

public class CreateSponsorshipRequestCommandHandler : IRequestHandler<CreateSponsorshipRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public CreateSponsorshipRequestCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }

    public async Task<int> Handle(CreateSponsorshipRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        var entity = new SponsorshipRequest
        {
            RequestorId = userId!,
            RequestorName = userName!,
            Title = request.Title,
            Department = request.Department,
            SponsorshipTypeId = request.SponsorshipTypeId,
            EventName = request.EventName,
            EventDate = request.EventDate,
            RequestedAmount = request.RequestedAmount,
            Purpose = request.Purpose,
            BusinessBenefit = request.BusinessBenefit,
            SupportingDocumentUrl = request.SupportingDocumentUrl,
            Status = SponsorshipRequestStatus.Draft
        };

        _context.SponsorshipRequests.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
