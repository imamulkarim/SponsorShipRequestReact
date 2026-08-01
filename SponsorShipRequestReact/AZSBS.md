In C#, there are three primary application patterns to consume messages from an Azure Service Queue (such as Azure Service Bus or Azure Queue Storage), and two retrieval modes handled via the official SDK.

---
https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk-2

The best choice depends on whether your project requires low-overhead background processing, an event-driven serverless architecture, or absolute manual control over the network requests

# 1. The 3 Architectural Implementation Patterns

## Event-Driven Serverless (Azure Functions)
This is the most popular, zero-boilerplate technique. You use a managed trigger to fire your C# code automatically whenever a new message lands in the queue.

- **How it works:** Uses the ServiceBusTrigger or QueueTrigger attributes.
- **Scale model:** Automatically scales out horizontally based on the queue length.
- **Code complexity:** Extremely low; Azure handles the polling infrastructure under the hood.
 
## Event-Driven Background Worker (ServiceBusProcessor)

Ideal for always-on containerized apps, microservices, or ASP.NET Core background worker services.

- **How it works:** Uses the high-level `ServiceBusProcessor` class from the `Azure.Messaging.ServiceBus` SDK. You register an event handler callback function.
- **Scale model:** Built-in concurrent processing through configuration properties.
- **Code complexity:** Medium; you must manage the lifecycle of the client host, but message retrieval loops are fully automated.

##Manual On-Demand Retrieval (ServiceBusReceiver / QueueClient)
A specialized low-level approach where your C# application explicitly controls exactly when and how many messages to read.

- **How it works:** Uses `ServiceBusReceiver.ReceiveMessagesAsync()` or `QueueClient.ReceiveMessagesAsync()`.
- **Scale model:** Manual. Your application must run dedicated loops or cron jobs.
- **Code complexity:** High; you have to handle continuous polling, network backoffs, and batch configurations manually.

---

# 2. The 2 Message Settlement / Retrieval Modes
When you choose to write custom C# applications (using the Receiver or Processor models), you must instruct Azure how to remove items from the queue using one of two modes:

- **Peek-Lock Mode (Default & Safe):** The consumer tells Azure to "lock" the message so other instances cannot see it. The application processes the data, and explicitly calls `CompleteMessageAsync()` to delete it from the queue. If your application crashes mid-process, the lock expires and the message safely becomes visible again.
- **Receive-and-Delete Mode:** Azure immediately deletes the message from the queue the millisecond it is read by your C# application. This offers higher throughput but sacrifices safety; if your consumer crashes while processing, the message data is permanently lost

# C# Code Examples
## A. The Processor Approach (Push-Style Web / Worker Apps)
```
using Azure.Messaging.ServiceBus;

string connectionString = "<CONNECTION_STRING>";
string queueName = "<QUEUE_NAME>";

await using var client = new ServiceBusClient(connectionString);
// The processor manages the background polling loop for you automatically
ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

// Register the handlers
processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;

await processor.StartProcessingAsync();

// Keep application alive while it consumes
Console.WriteLine("Press any key to end the processing");
Console.ReadKey();

async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // Complete the message to clear it from the queue (Peek-Lock pattern)
    await args.CompleteMessageAsync(args.Message);
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

```

## B. The Receiver Approach (Manual Pull-Style Batching)

```
using Azure.Messaging.ServiceBus;

string connectionString = "<CONNECTION_STRING>";
string queueName = "<QUEUE_NAME>";

await using var client = new ServiceBusClient(connectionString);
ServiceBusReceiver receiver = client.CreateReceiver(queueName);

// Pull up to a maximum of 10 messages from the queue at once
IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10);

foreach (var message in receivedMessages)
{
    Console.WriteLine($"Processing message: {message.Body}");
    // Complete individual message settlement
    await receiver.CompleteMessageAsync(message);
}

```


- Does Queues maintain an ordered message(data) 
    - The short answer is: Service Bus stores messages in the order they arrive, but it does not guarantee they will be processed in that same order unless you explicitly use Sessions.

