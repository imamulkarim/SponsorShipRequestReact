using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Infrastructure.Services.SponsorshipFinance.Models;

internal record SponsorShipApproveApiRequest
{
    public string id { get; set; } = null!;
    public string expenseDescription { get; set; } = null!;
    public decimal amount { get; set; }
    public string? create_by { get; set; }
    public DateTimeOffset createdAt { get; set; }
}
