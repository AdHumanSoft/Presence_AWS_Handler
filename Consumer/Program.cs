using Amazon.SimpleEmail;
using Amazon.SQS;
using Consumer;
using Consumer.Utils;


Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2");
Environment.SetEnvironmentVariable("AWS_PROFILE", "sqs-test");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<SqsConsumerService>();
builder.Services.AddSingleton<IAmazonSQS>(sp => new AmazonSQSClient(
    Amazon.RegionEndpoint.EUWest2
));
builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddSingleton<IAmazonSimpleEmailService>(sp => new AmazonSimpleEmailServiceClient(   Amazon.RegionEndpoint.EUWest2
));
builder.Services.AddSingleton<Emailer>(x => new Emailer(x.GetRequiredService<IAmazonSimpleEmailService>()));

builder.Services.AddMessageHandlers();



 



var app = builder.Build();

app.Run();
