using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging;

public class SnsClientFactory : ISnsClientFactory
{
    private readonly IOptions<TopicSettings> _topicSettings;
    private readonly IAmazonSimpleNotificationService _sns;
    private string? _topicArn;
    public SnsClientFactory(IAmazonSimpleNotificationService simpleNotificationService, IOptions<TopicSettings> topicSettings)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(topicSettings));

        _topicSettings = topicSettings;
        _sns = simpleNotificationService;
    }

    private async Task<string> GetTopicArnAsync()
    {
        if (_topicArn is not null)
        {
            return _topicArn;
        }

        var queueUrlResponse = await _sns.FindTopicAsync(_topicSettings.Value.Name);
        _topicArn = queueUrlResponse.TopicArn;

        return _topicArn;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message)
    {
        var topicArn = await GetTopicArnAsync();

        var sendMessageRequest = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await _sns.PublishAsync(sendMessageRequest);
    }
}