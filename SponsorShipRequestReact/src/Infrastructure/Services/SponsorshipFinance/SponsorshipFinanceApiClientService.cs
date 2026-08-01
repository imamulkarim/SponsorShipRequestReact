using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;
using TechAssessment.Domain.Events;
using TechAssessment.Infrastructure.Services.SponsorshipFinance.Models;

namespace TechAssessment.Infrastructure.Services.SponsorshipFinance;

public class SponsorshipFinanceApiClientService : ISponsorshipFinanceApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SponsorshipFinanceApiClientService> _logger;

    public SponsorshipFinanceApiClientService(HttpClient httpClient, ILogger<SponsorshipFinanceApiClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> PostSponsorshipRequestAmount<T>(T sponsorshipData, CancellationToken cancellationToken = default)
    {
        try
        {
            var sponsorApprovedEventData = sponsorshipData as SponsorShipApprovedEvent;
            if(sponsorApprovedEventData == null)
                throw new ArgumentException("Invalid sponsorship data type. Expected SponsorShipApprovedEvent.", nameof(sponsorshipData));

            var sponsorshipRequest = new SponsorShipApproveApiRequest
            {
                id = sponsorApprovedEventData.FinanceApprovedEvent.RequestorId,
                expenseDescription = sponsorApprovedEventData.FinanceApprovedEvent.EventName,
                amount = sponsorApprovedEventData.FinanceApprovedEvent.RequestedAmount,
                create_by = sponsorApprovedEventData.FinanceApprovedEvent.CreatedBy,
                createdAt = sponsorApprovedEventData.FinanceApprovedEvent.Created
            };

            var response = await _httpClient.PostAsJsonAsync("/api/SponsorshipExpensess", sponsorshipRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during the third-party API call.");
            throw;
        }

    }
}
