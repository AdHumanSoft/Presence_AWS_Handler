using System.Reflection;
using Amazon.SimpleEmail;
using Amazon.SQS;
using Consumer.Handlers;
using Consumer.Firebase;
using Serilog;
using Google.Cloud.Firestore;

namespace Consumer.Extensions;


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
        Environment.SetEnvironmentVariable("AWS_PROFILE", "default");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "/home/kedalen/Downloads/fire.json");
        services.AddSingleton<IAmazonSimpleEmailService>(sp => new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.EUWest2));
        services.AddSingleton<IAmazonSQS>(sp => new AmazonSQSClient(Amazon.RegionEndpoint.EUWest2));
        services.AddSingleton<FirestoreDb>(FirestoreDb.Create("adhumansoft-schools"));
        services.AddSingleton<IDbHandler, DbHandler>();
        return services;
    }


    //Add Serilog to the service collection
    public static IHostBuilder AddSerilog(this IHostBuilder services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e => e.Level >= Serilog.Events.LogEventLevel.Warning)
            .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
            
            .WriteTo.Logger(lc => 
            lc.Filter.ByIncludingOnly(e => e.Level < Serilog.Events.LogEventLevel.Fatal)
            .WriteTo.Console())

            .CreateLogger();

        services.UseSerilog(logger);
        return services;
    }

}