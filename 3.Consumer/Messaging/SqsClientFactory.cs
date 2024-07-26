using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Customers.Consumer.Messaging
{
    public class SqsClientFactory : ISqsClientFactory
    {
        private readonly IOptions<QueueSettings> _queueSettings;
        private readonly IAmazonSQS _sqs;
        private string? _queueUrl;

        public SqsClientFactory(IAmazonSQS amazonSQS, IOptions<QueueSettings> queueSettings)
        {
            ArgumentException.ThrowIfNullOrEmpty("queueSettings", nameof(queueSettings));

            _queueSettings = queueSettings;
            _sqs = amazonSQS;
        }

        public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken stoppingToken)
        {
            return await _sqs.SendMessageAsync(sendMessageRequest, stoppingToken);
        }

        public async Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken stoppingToken)
        {
            return await _sqs.SendMessageAsync(queueUrl, messageBody, stoppingToken);
        }

        public async Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken stoppingToken)
        {
            return await _sqs.DeleteMessageAsync(queueUrl, receiptHandle, stoppingToken);
        }

        public async Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest deleteMessageRequest, string receiptHandle, CancellationToken stoppingToken)
        {
            return await _sqs.DeleteMessageAsync(deleteMessageRequest, stoppingToken);
        }

        public async Task<string> GetQueueUrlAsync(CancellationToken stoppingToken)
        {
            if (_queueUrl is not null)
            {
                return _queueUrl;
            }

            var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);
            _queueUrl = queueUrlResponse.QueueUrl;
            return _queueUrl;
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken stoppingToken)
        {
            return await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageAsync(CancellationToken stoppingToken)
        {
            return await _sqs.ReceiveMessageAsync(_queueSettings.Value.Name, stoppingToken);
        }
    }
}