using System.Reflection;
using Consumer.Handlers;
using Serilog;

namespace Consumer;


public static class HandlerExtensions {

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services) {
        var handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            
        foreach (var handler in handlers)
        {   
            var serviceDescriptor = new ServiceDescriptor(handler.AsType(), handler, ServiceLifetime.Scoped);
            services.Add(serviceDescriptor);   
        }

        Console.WriteLine("MessageHandlers added successfully {0}", handlers.Select(x => x.Name).Aggregate((a, b) => a + ", " + b));
        return services;
    }




}