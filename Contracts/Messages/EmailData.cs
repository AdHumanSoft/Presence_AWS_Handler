namespace Contracts.Messages;

using System.Text.Json.Serialization;




public class EmailData {
    
    [JsonPropertyName("members")]
    public string[] Members { get; init; } = new string[0];

    [JsonPropertyName("subject")]
    public string Subject { get; init; } = string.Empty;


    //This can be null 
    [JsonPropertyName("template")]
    public string? template { get; init; } = null;

}