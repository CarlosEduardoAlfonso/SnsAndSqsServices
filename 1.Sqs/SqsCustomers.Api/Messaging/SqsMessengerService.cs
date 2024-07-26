using Amazon.SQS.Model;
using System.Text.Json;

namespace Customers.Api.Messaging;

public class SqsMessengerService : ISqsMessengerService
{
    private readonly ISqsClientFactory _sqsClientFactory;

    public SqsMessengerService(ISqsClientFactory sqsClientFactory)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(sqsClientFactory));
        _sqsClientFactory = sqsClientFactory;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message, CancellationToken cancellationToken)
    {
        var queueUrl = await _sqsClientFactory.GetQueueUrlAsync(cancellationToken);

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
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

        return await _sqsClientFactory.SendMessageAsync(sendMessageRequest, cancellationToken);
    }
}
