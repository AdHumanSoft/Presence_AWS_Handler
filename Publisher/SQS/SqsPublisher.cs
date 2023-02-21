using System.Text.Json;
using Amazon.Sqs;
using Amazon.SQS;

using Contracts.Messages;

namespace Publisher;


public class SqsPublisher {
    private readonly IAmazonSQS _sqsClient;

    public SqsPublisher(IAmazonSQS sqsClient) {
        _sqsClient = sqsClient;
    }

    public async Task PublishAsync<TMessage>(TMessage message, string queueName) 
    where TMessage : IMessage
    {
        var queueUrl = await _sqsClient.GetQueueUrlAsync(queueName);

        var sendRequest = new Amazon.SQS.Model.SendMessageRequest{
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, Amazon.SQS.Model.MessageAttributeValue>{
                {
                    "MessageType",
                    new  Amazon.SQS.Model.MessageAttributeValue {
                        DataType = "String",
                        StringValue = message.MessageType
                    }
                }
            }
        };
     

        var result = await _sqsClient.SendMessageAsync(sendRequest);

        Console.WriteLine($"Published message to queue\n{queueName}");
        Console.WriteLine($"Status:\n {result.HttpStatusCode}");
    }
}