namespace Customers.Api.Messaging;

public class QueueSettings
{
    public const string Key = "Queue";
    public string? Name { get; set; }
    public string? Region { get; set; }
    public string? QueueId { get; set; }
    public string? IamAccessKey { get; set; }
    public string? IamSecretKey { get; set; }
}
