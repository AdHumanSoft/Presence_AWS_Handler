using System.Text.Json.Serialization;

namespace Contracts.Messages;

public interface IMessage 
{
    [JsonIgnore]
    public string MessageType { get;  }
}