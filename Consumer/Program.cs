using Amazon.SimpleEmail;
using Consumer.Aws;
using Consumer.Extensions;
using Consumer.Firebase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAws();
builder.Host.AddSerilog();
builder.Services.AddHostedService<SQSConsumerService>();
builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddSingleton<Emailer>(x => new Emailer(x.GetRequiredService<IAmazonSimpleEmailService>()));
builder.Services.AddMessageHandlers();







var app = builder.Build();






app.Run();


