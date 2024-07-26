using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using Customers.Consumer.Messaging;
using MediatR;
using System.Text.Json;

namespace Customers.Consumer
{

    public class QueueConsumerService : BackgroundService
    {
        private readonly ISqsClientFactory sqsClientFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<QueueConsumerService> _logger;

        public QueueConsumerService(ISqsClientFactory sqsClientFactory, IMediator mediator, ILogger<QueueConsumerService> logger)
        {
            this.sqsClientFactory = sqsClientFactory;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueUrlResponse = await sqsClientFactory.GetQueueUrlAsync(stoppingToken);

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse,
                AttributeNames = new List<string> { "All" },
                MessageAttributeNames = new List<string> { "All" },
                MaxNumberOfMessages = 1
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await sqsClientFactory.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                foreach (var message in response.Messages)
                {
                    var messageType = message.MessageAttributes["MessageType"].StringValue;
                    var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                    if (type is null)
                    {
                        _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                        continue;
                    }

                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                    try
                    {
                        await _mediator.Send(typedMessage, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Message failed during processing");
                        continue;
                    }

                    await sqsClientFactory.DeleteMessageAsync(queueUrlResponse, message.ReceiptHandle, stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}