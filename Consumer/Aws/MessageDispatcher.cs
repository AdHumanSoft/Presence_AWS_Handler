using System.Reflection;
using Consumer.Handlers;
using Contracts.Messages;

namespace Consumer.Aws;

public class MessageDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly Dictionary<string, Type> _messageMappings;/* = new()
    {
        { nameof(CustomerCreated), typeof(CustomerCreated) },
        { nameof(CustomerDeleted), typeof(CustomerDeleted) }
    };*/

    private readonly Dictionary<string, Func<IServiceProvider, IMessageHandler>> _handlers;/* = new()
    {
        { nameof(CustomerCreated), provider => provider.GetRequiredService<CustomerCreatedHandler>() },
        { nameof(CustomerDeleted), provider => provider.GetRequiredService<CustomerDeletedHandler>() }
    };*/

    public MessageDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;


        //Find the IMessage implementations in the Contracts project

        _messageMappings = typeof(IMessage).Assembly.DefinedTypes
            .Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary(info => info.Name, info => info.AsType());


        _handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary<TypeInfo, string, Func<IServiceProvider, IMessageHandler>>(
            info => ((Type)info.GetProperty(nameof(IMessageHandler.MessageType))!.GetValue(null)!)!.Name,
            info => provider => (IMessageHandler)provider.GetRequiredService(info.AsType()));


    }

    public async Task DispatchAsync<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = _handlers[message.MessageType](scope.ServiceProvider);
        await handler.HandleAsync(message);
    }

    private bool CanHandleMessageType(string messageTypeName)
    {
        return _handlers.ContainsKey(messageTypeName);
    }


    public bool TryGetType(string messageTypeName, out Type messageType)
    {



        if (string.IsNullOrWhiteSpace(messageTypeName))
        {
            messageType = default!;
            return false;
        }

        if (!CanHandleMessageType(messageTypeName))
        {
            messageType = default!;
            return false;
        }
        var res = GetMessageTypeByName(messageTypeName);
        if (res is null)
        {
            messageType = default!;
            return false;
        }


        messageType = res;
        return true;
    }
    private Type? GetMessageTypeByName(string messageTypeName)
    {
        return _messageMappings.GetValueOrDefault(messageTypeName);
    }


}