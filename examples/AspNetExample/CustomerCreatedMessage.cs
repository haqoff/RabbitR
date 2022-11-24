using System.Text.Json.Serialization;
using Rabbiter.Messages;

namespace AspNetExample;

public class CustomerCreatedMessage : IEventBusMessage
{
    public const string ExchangeName = "customer_created_exchange";

    [JsonConstructor]
    public CustomerCreatedMessage(int id, string fullName, DateTime createDate)
    {
        Id = id;
        FullName = fullName;
        CreateDate = createDate;
    }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("fullName")]
    public string FullName { get; init; }

    [JsonPropertyName("updateDate")]
    public DateTime CreateDate { get; init; }
}