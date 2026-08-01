using TechAssessment.Domain.Entities;

namespace TechAssessment.Application.Common.Interfaces;

public interface IApplicationDbContext
{

    DbSet<SponsorshipRequest> SponsorshipRequests { get; }

    DbSet<SponsorshipRequestApproval> SponsorshipRequestApprovals { get; }

    DbSet<SponsorshipType> SponsorshipTypes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
