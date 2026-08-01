using System;
using System.Collections.Generic;
using System.Text;
using Azure.Messaging.ServiceBus;

namespace MessagingServices;

public class AzureServiceBusPublisher : IMessagePublisher
{

    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    // Pass either your Queue name or your Topic name into 'queueOrTopicName'
    public AzureServiceBusPublisher()
    {
        //string connectionString = builder
        //    string queueOrTopicName;
        _client = new ServiceBusClient("connectionString");
        _sender = _client.CreateSender("queueOrTopicName");
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }

    public async Task SendOrderMessageAsync(string messageId, string messageDetails)
    {
        // 1. Create the message payload
        ServiceBusMessage message = new ServiceBusMessage(messageDetails)
        {
            // 2. CRITICAL FOR AUTO-SCALE DUPLICATE DETECTION: 
            // If a network glitch occurs and this sends twice, 
            // Azure Service Bus will discard the second one based on this ID.
            MessageId = $"order-{messageId}",
            ContentType = "application/json"
        };

        // 3. Send the message
        await _sender.SendMessageAsync(message);
        Console.WriteLine($"Sent message with ID: {message.MessageId}");
    }
}
