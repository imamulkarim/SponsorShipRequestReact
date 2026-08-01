using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Domain.Events;

public class SponsorShipApprovedEvent : BaseEvent
{
    public SponsorshipRequest FinanceApprovedEvent { get; }

    public SponsorShipApprovedEvent(SponsorshipRequest financeApprovedEvent)
    {
        FinanceApprovedEvent = financeApprovedEvent;
    }
}
