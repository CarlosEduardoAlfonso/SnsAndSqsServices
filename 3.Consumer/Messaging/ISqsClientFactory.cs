using Amazon.SQS.Model;

namespace Customers.Consumer.Messaging
{
    public interface ISqsClientFactory
    {
        Task<string> GetQueueUrlAsync(CancellationToken stoppingToken);
        Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken stoppingToken);
        Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken stoppingToken);
        Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken stoppingToken);
        Task<ReceiveMessageResponse> ReceiveMessageAsync(CancellationToken stoppingToken);
        Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken stoppingToken);
        Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest deleteMessageRequest, string receiptHandle, CancellationToken stoppingToken);
    }
}