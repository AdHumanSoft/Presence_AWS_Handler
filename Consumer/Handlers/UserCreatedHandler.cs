using Contracts.Messages;
using Consumer.Aws;
using Consumer.Firebase;

namespace Consumer.Handlers;
public class UserCreatedHandler : IMessageHandler
{
    private readonly ILogger<UserCreatedHandler> _logger;

    private readonly IDbHandler _dbHandler;
    private readonly Emailer _emailer;

    public UserCreatedHandler(ILogger<UserCreatedHandler> logger , IDbHandler _db,Emailer emailer)
    {
        _logger = logger;
        _emailer = emailer;
        _dbHandler = _db;
    }
    public async Task HandleAsync(IMessage message)
    {
        var body = (UserCreated)message;
        //From the body we need to query the database to get the member details and then send an email to the member with the password
        
        var admin = await _dbHandler.GetAdminAsync(body.UserId);
        if (admin == null)
        {
            _logger.LogError("Admin {0} not found", body.UserId);
            return;
        }
        
        var options = new UserCreatedEmailOptions
        {
            Name = admin.Name,
            UserName = body.Email,
            Password = body.Password
        };
        await _emailer.SendUserCreatedEmailAsync(options);
    }

    public static Type MessageType { get; } = typeof(UserCreated);
}
