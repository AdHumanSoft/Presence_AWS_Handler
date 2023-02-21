using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Contracts.Messages;

namespace Consumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly MessageDispatcher _dispatcher;
    private const string QueueName = "EmailingService";
    private readonly List<string> _messageAttributeNames = new() { "All" };

    public SqsConsumerService(IAmazonSQS sqs, MessageDispatcher dispatcher)
    {
        _sqs = sqs;
        _dispatcher = dispatcher;
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

            foreach (var message in messageResponse.Messages)
            {
                var messageTypeName = message.MessageAttributes
                    .GetValueOrDefault(nameof(IMessage.MessageType))?.StringValue;

                if (messageTypeName is null)
                {
                    continue;
                }

                if (!_dispatcher.CanHandleMessageType(messageTypeName))
                {
                    continue;
                }

                var messageType = _dispatcher.GetMessageTypeByName(messageTypeName);
                if(messageType is null) {
                    Console.WriteLine($"Could not find type {messageTypeName}");
                    continue;
                }
                try {
                var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;
                Console.WriteLine($"Processing message {message.Body}");
                await _dispatcher.DispatchAsync(messageAsType);
                await _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, ct);
                } catch (Exception e) {

                    Console.WriteLine($"Error: {e.Message} while processing message {message.Body}");
                }
            }
        }
    }
}