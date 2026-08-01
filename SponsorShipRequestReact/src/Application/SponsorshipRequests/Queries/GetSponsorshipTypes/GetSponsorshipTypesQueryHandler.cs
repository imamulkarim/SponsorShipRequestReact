using Microsoft.EntityFrameworkCore;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application.SponsorshipRequests.Queries.GetSponsorshipTypes;

public class GetSponsorshipTypesQueryHandler : IRequestHandler<GetSponsorshipTypesQuery, SponsorshipTypesVm>
{
    private readonly IApplicationDbContext _context;

    public GetSponsorshipTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SponsorshipTypesVm> Handle(GetSponsorshipTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _context.SponsorshipTypes
            .Where(x => x.IsActive)
            .Select(x => new SponsorshipTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return new SponsorshipTypesVm { SponsorshipTypes = types };
    }
}
