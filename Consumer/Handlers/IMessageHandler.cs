using Contracts.Messages;

namespace Consumer.Handlers;

public interface IMessageHandler
{
    public Task HandleAsync(IMessage message);
    
    public static Type MessageType { get; } = typeof(IMessage);
}