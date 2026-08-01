using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.Common.Interfaces;

public interface ISponsorshipFinanceApiClientService
{
    public Task<bool> PostSponsorshipRequestAmount<T>(T sponsorshipData, CancellationToken cancellationToken = default);
}
