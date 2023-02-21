using Contracts.Messages;

namespace Consumer.Handlers;
public class MemberQrReadHandler : IMessageHandler
{
   private readonly ILogger<MemberQrReadHandler> _logger;

    public MemberQrReadHandler(ILogger<MemberQrReadHandler> logger)
    {
        _logger = logger;
    }
    public async Task HandleAsync(IMessage message)
    {
        var body = (MemberQrRead)message;
        //From the body we need to query the database to get the member details and then send an email to the member
        await Task.CompletedTask;
    }

    public static Type MessageType {get;} = typeof(MemberQrRead);
}
