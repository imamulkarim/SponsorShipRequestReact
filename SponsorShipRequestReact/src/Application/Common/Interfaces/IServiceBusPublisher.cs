using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.Common.Interfaces;

public interface IServiceBusPublisher 
{
    Task PublishAsync<T>(string messageId,T message,string topic_queue, CancellationToken cancellationToken = default);
}
