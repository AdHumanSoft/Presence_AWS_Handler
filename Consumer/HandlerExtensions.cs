using System.Reflection;
using Amazon.SimpleEmail;
using Amazon.SQS;
using Consumer.Handlers;
using Serilog;

namespace Consumer;


public static class HandlerExtensions
{


    /// <summary>
    /// Add all message handlers to the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
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



    /// <summary>
    /// Add AWS services to the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static IServiceCollection AddAws(this IServiceCollection services)
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2");
        Environment.SetEnvironmentVariable("AWS_PROFILE", "sqs-test");
        services.AddSingleton<IAmazonSimpleEmailService>(sp => new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.EUWest2));
        services.AddSingleton<IAmazonSQS>(sp => new AmazonSQSClient(Amazon.RegionEndpoint.EUWest2));
        return services;
    }


}