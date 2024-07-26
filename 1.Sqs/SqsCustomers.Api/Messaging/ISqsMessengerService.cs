using Amazon.SQS.Model;

namespace Customers.Api.Messaging;

public interface ISqsMessengerService
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message, CancellationToken cancellationToken);
}
