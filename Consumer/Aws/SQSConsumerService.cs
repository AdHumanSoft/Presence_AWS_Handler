using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net.Http;
using Contracts.Messages;

namespace Consumer.Aws;

public class SQSConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;

    private readonly ILogger<SQSConsumerService> _logger;
    private readonly MessageDispatcher _dispatcher;
    private const string QueueName = "EmailingService";
    private readonly List<string> _messageAttributeNames = new() { "All" };

    public SQSConsumerService(IAmazonSQS sqs, MessageDispatcher dispatcher, ILogger<SQSConsumerService> logger)
    {
        _sqs = sqs;
        _dispatcher = dispatcher;
        _logger = logger;
    }



    protected override async Task ExecuteAsync(CancellationToken ct)
    {


        
        var queueUrl = await _sqs.GetQueueUrlAsync(QueueName, ct);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _messageAttributeNames
        };

        while (!ct.IsCancellationRequested)
        {
            var messageResponse = await _sqs.ReceiveMessageAsync(receiveRequest, ct);
            if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                //Do some logging or handling?
                continue;
            }
            var messagesToDelete = new List<DeleteMessageBatchRequestEntry>();
            foreach (var message in messageResponse.Messages)
            {
                var messageTypeName = message.MessageAttributes
                    .GetValueOrDefault(nameof(IMessage.MessageType))?.StringValue;

                if (messageTypeName is null)
                {
                    _logger.LogWarning("Message {Message} has no message type", message.Body);
                    continue;
                }


                if (!_dispatcher.TryGetType(messageTypeName, out var messageType))
                {
                    _logger.LogWarning("Unknown message type {MessageType}", messageTypeName);
                    continue;
                }

                try
                {
                    var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;
                    _logger.LogInformation("Processing message {Message}", messageAsType);
                    await _dispatcher.DispatchAsync(messageAsType);
                    messagesToDelete.Add(new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle));

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing message {Message}", message.Body);
                }
            }

            if (messagesToDelete.Any())
            {
                var deleteRequest = new DeleteMessageBatchRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    Entries = messagesToDelete
                };
                await _sqs.DeleteMessageBatchAsync(deleteRequest, ct);
            }
        }
    }
}