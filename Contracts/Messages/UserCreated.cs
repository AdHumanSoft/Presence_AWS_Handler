using System.Text.Json.Serialization;

namespace Contracts.Messages;



public class UserCreated : IMessage
{   
    [JsonIgnore]
    public string MessageType => nameof(UserCreated);

    [JsonPropertyName("userId")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;
}