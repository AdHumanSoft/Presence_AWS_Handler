using System.Diagnostics.Contracts;
using Contracts.Messages;
using Consumer.Firebase;
using Consumer.Aws;

namespace Consumer.Handlers;
public class MemberQrReadHandler : IMessageHandler
{
    private readonly ILogger<MemberQrReadHandler> _logger;
    private readonly IDbHandler _dbHandler;

    private readonly Emailer _emailer;

    public MemberQrReadHandler(ILogger<MemberQrReadHandler> logger, IDbHandler dbHandler, Emailer emailer)
    {
        _logger = logger;
        _dbHandler = dbHandler;
        _emailer = emailer;
    }

    private async Task SendEmailToContact(string contactId, string from,QrReadEmailOptions opts)
    {
        var contact = await _dbHandler.GetContactAsync(contactId);
        if (contact is null)
        {
            _logger.LogError("Contact {0} not found", contactId);
            return;
        }
        await _emailer.SendQrReadEmailAsync(from, contact.Email, opts);

    }
            

    public async Task HandleAsync(IMessage message)
    {
        var body = (MemberQrRead)message;
        //From the body we need to query the database to get the member details and then send an email to the member

        var entity = await _dbHandler.GetEntityAsync(body.EntityId);
        var center = await _dbHandler.GetCenterAsync(body.EntityId, body.CenterId);
        var member = await _dbHandler.GetMemberAsync(body.EntityId, body.CenterId, body.MemberId);

        if (member == null)
        {
            _logger.LogError("Member {0} not found", body.MemberId);
            return;
        }
        var options = new QrReadEmailOptions
        {
            Date = body.DateOfArrival.ToString(),
            Name = member.Name,
        };
        var contacts = member.ContactRules.OnQrRead.Select(x => SendEmailToContact(x, "hyrlimbo@gmail.com", options));
        await Task.WhenAll(contacts);

        await Task.CompletedTask;
    }

    public static Type MessageType { get; } = typeof(MemberQrRead);
}
