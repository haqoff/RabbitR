using Rabbiter.Messages;
using System.Text.Json.Serialization;

namespace AspNetExample;

public class CreateCustomerRequestMessage : IEventBusMessage
{
    [JsonConstructor]
    public CreateCustomerRequestMessage(string fullName)
    {
        FullName = fullName;
    }


    [JsonPropertyName("fullName")]
    public string FullName { get; init; }
}