namespace MessagingServices;

public interface IMessagePublisher : IAsyncDisposable
{
    Task SendOrderMessageAsync(string messageId, string messageDetails);
}
