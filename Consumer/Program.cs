using Amazon.SimpleEmail;
using Amazon.SQS;
using Consumer;
using Consumer.Utils;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAws();
builder.Services.AddHostedService<SqsConsumerService>();
builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddSingleton<Emailer>(x => new Emailer(x.GetRequiredService<IAmazonSimpleEmailService>()));

builder.Services.AddMessageHandlers();



 



var app = builder.Build();

app.Run();
