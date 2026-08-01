using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Infrastructure.Messaging;

public class AzureServiceBusPublisher : IServiceBusPublisher
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<AzureServiceBusPublisher> _logger;


    public AzureServiceBusPublisher(ServiceBusClient serviceBusClient,
        ILogger<AzureServiceBusPublisher> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }


    public async Task PublishAsync<T>(string messageId,T message, string topicQueue, CancellationToken cancellationToken = default)
    {
        try
        {

            // Create sender once per topic/queue (or reuse if same)
            await using var sender = _serviceBusClient.CreateSender(topicQueue);
               
            var messageBody = JsonSerializer.Serialize(message);
            //await sender.SendMessageAsync(
            //    new ServiceBusMessage(messageBody),
            //    cancellationToken);

            // 1. Create the message payload
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                // 2. CRITICAL FOR AUTO-SCALE DUPLICATE DETECTION: 
                // If a network glitch occurs and this sends twice, 
                // Azure Service Bus will discard the second one based on this ID.
                MessageId = $"order-{messageId}",
                ContentType = "application/json",
                Subject = typeof(T).Name,
            };

            // 3. Send the message
            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            _logger.LogInformation($"Azure: Message Published: {messageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message");
            throw;
        }
    }
}
