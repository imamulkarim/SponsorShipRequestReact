using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Application.Common.Models;

namespace TechAssessment.Application.SponsorshipRequests.Commands.ApproveRequest;

public record ApproveRequestCommand(int Id, string Remarks, string ApproverRole) : IRequest<Result<int>>;
