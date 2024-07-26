namespace Customers.Api.Messaging;

public class TopicSettings
{
    public const string Key = "Topic";

    public string? Name { get; set; }
    public string? Region { get; set; }
    public string? QueueId { get; set; }
    public string? IamAccessKey { get; set; }
    public string? IamSecretKey { get; set; }
}
