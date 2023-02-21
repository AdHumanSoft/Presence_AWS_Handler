using System.Text.Json.Serialization;


namespace Contracts.Messages;



public class MemberQrRead : IMessage
{
    [JsonIgnore]
    public string MessageType => nameof(MemberQrRead);
    
    [JsonPropertyName("emailData")]
    public EmailData EmailData { get; init; } = new EmailData();

    [JsonPropertyName("memberId")]
    public string MemberId { get; init; } = string.Empty;

    [JsonPropertyName("centerId")]
    public string CenterId { get; init; } = string.Empty;
    //Date as ISO 8601
    [JsonPropertyName("dateOfArrival")]
    public string DateOfArrival { get; init; } = string.Empty;
}