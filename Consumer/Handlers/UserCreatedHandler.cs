using Contracts.Messages;
using Consumer.Utils;

namespace Consumer.Handlers;
public class UserCreatedHandler : IMessageHandler
{
    private readonly ILogger<UserCreatedHandler> _logger;

    private readonly Emailer _emailer;

    public UserCreatedHandler(ILogger<UserCreatedHandler> logger , Emailer emailer)
    {
        _logger = logger;
        _emailer = emailer;
    }
    public async Task HandleAsync(IMessage message)
    {
        var body = (UserCreated)message;
        //From the body we need to query the database to get the member details and then send an email to the member with the password

        var options = new UserCreatedEmailOptions
        {
            Name = body.UserId,
            UserName = body.Email,
            Password = body.Password
        };
        await _emailer.SendUserCreatedEmailAsync(options);
    }

    public static Type MessageType { get; } = typeof(UserCreated);
}
