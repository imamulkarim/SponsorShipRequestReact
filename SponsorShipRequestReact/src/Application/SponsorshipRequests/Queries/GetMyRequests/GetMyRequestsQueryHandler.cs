using Microsoft.EntityFrameworkCore;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;

public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, MyRequestsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyRequestsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<MyRequestsVm> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _context.SponsorshipRequests
            .Where(x => x.RequestorId == _user.Id)
            .OrderByDescending(x => x.Created)
            .Select(x => new SponsorshipRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Department = x.Department,
                SponsorshipType = x.SponsorshipTypeId.ToString(),
                EventName = x.EventName,
                EventDate = x.EventDate,
                RequestedAmount = x.RequestedAmount,
                Purpose = x.Purpose,
                Status = x.Status.ToString(),
                CreatedAt = x.Created.DateTime,
                LastModifiedAt = x.LastModified.DateTime 
            })
            .ToListAsync(cancellationToken);

        return new MyRequestsVm { Requests = requests };
    }
}
