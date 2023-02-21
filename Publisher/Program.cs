using System.Globalization;
using Publisher;
using Contracts.Messages;

Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2");
Environment.SetEnvironmentVariable("AWS_PROFILE", "sqs-test");


var client = new Amazon.SQS.AmazonSQSClient(Amazon.RegionEndpoint.EUWest2);

var t = await client.ListQueuesAsync("");
foreach (var queue in t.QueueUrls)
{
    Console.WriteLine(queue);
}

var publisher = new SqsPublisher(client);



var guids = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid().ToString()).ToArray();
var now = DateTime.Now;
//Convert now to ISOString
var d = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);

var i = 0;

while (true)
{
    Console.WriteLine("Press any key to publis a message");
    Console.ReadKey();
    var message = new UserCreated{
        Password = "password",
        Email = i % 2 == 0 ? "jpujolcarme@gmail.com" : "hyrlimbo@gmail.com",
        UserId = guids[new Random().Next(0, guids.Length - 1)]
    };

    await publisher.PublishAsync<UserCreated>(message, "EmailingService");

    Console.WriteLine($"Published message {message.UserId} at {d}");


    i++;
    Console.Clear();
}
