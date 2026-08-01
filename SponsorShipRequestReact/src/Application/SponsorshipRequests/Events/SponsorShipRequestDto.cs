using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Application.SponsorshipRequests.Queries.GetRequestDetail;

namespace TechAssessment.Application.SponsorshipRequests.Events;

internal class SponsorShipRequestDto
{
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string RequestorName { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string SponsorshipType { get; set; } = null!;
        public string EventName { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string Purpose { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? ManagerApprovalRemarks { get; set; }
        public string? FinanceApprovalRemarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
}
