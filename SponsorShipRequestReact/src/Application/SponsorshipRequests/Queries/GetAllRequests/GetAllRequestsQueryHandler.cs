using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;

namespace TechAssessment.Application.SponsorshipRequests.Queries.GetAllRequests;

public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, AllRequestsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetAllRequestsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<AllRequestsVm> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _context.SponsorshipRequests
            .OrderByDescending(x => x.Created)
            .Select(x => new AllSponsorshipRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Department = x.Department,
                SponsorshipType = x.SponsorshipTypeId.ToString(),
                //EventName = x.EventName,
                EventDate = x.EventDate,
                RequestedAmount = x.RequestedAmount,
                //Purpose = x.Purpose,
                Status = x.Status.ToString(),
                CreatedAt = x.Created.DateTime,
                //LastModifiedAt = x.LastModified.DateTime
            })
            .ToListAsync(cancellationToken);

        return new AllRequestsVm { Requests = requests };
    }
}
