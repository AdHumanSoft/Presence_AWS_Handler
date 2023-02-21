using System.Reflection;
using Consumer.Handlers;
using Contracts.Messages;

namespace Consumer;

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

        Console.WriteLine("MessageMappings added successfully {0}", _messageMappings.Count());

        _handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary<TypeInfo, string, Func<IServiceProvider, IMessageHandler>>(
            info => ((Type)info.GetProperty(nameof(IMessageHandler.MessageType))!.GetValue(null)!)!.Name,
            info => provider => (IMessageHandler)provider.GetRequiredService(info.AsType()));

        //Print the name of all the _messageMappings
        foreach (var messageMapping in _messageMappings)
        {
            Console.WriteLine("MessageMapping: {0}", messageMapping.Key);
        }

        //Print the name of all the _handlers
        foreach (var handler in _handlers)
        {
            Console.WriteLine("Handler: {0}", handler.Key);
        }
    }

    public async Task DispatchAsync<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = _handlers[message.MessageType](scope.ServiceProvider);
        await handler.HandleAsync(message);
    }

    public bool CanHandleMessageType(string messageTypeName)
    {
        return _handlers.ContainsKey(messageTypeName);
    }

    public Type? GetMessageTypeByName(string messageTypeName)
    {

        var n = _messageMappings.GetValueOrDefault(messageTypeName) is null ? "null" : _messageMappings.GetValueOrDefault(messageTypeName)!.Name;

        return _messageMappings.GetValueOrDefault(messageTypeName);
    
    }
}