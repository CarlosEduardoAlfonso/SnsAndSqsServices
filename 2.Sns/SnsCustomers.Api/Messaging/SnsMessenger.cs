using Amazon.SimpleNotificationService.Model;

namespace Customers.Api.Messaging;

public class SnsMessenger : ISnsMessenger
{
    private readonly ISnsClientFactory snsClientFactory;

    public SnsMessenger(ISnsClientFactory snsClientFactory)
    {
        this.snsClientFactory = snsClientFactory;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message)
    {
        return await snsClientFactory.PublishMessageAsync<T>(message);
    }
}
