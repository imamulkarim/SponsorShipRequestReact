using System.Reflection;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Domain.Entities;
using TechAssessment.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TechAssessment.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SponsorshipRequest> SponsorshipRequests => Set<SponsorshipRequest>();

    public DbSet<SponsorshipRequestApproval> SponsorshipRequestApprovals => Set<SponsorshipRequestApproval>();

    public DbSet<SponsorshipType> SponsorshipTypes => Set<SponsorshipType>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
