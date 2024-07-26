using Amazon;
using Amazon.SQS;
using Customers.Consumer;
using Customers.Consumer.Messaging;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));

var queueSettings = new QueueSettings();
builder.Configuration.GetRequiredSection(QueueSettings.Key).Bind(queueSettings);

builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>(_ =>
    new AmazonSQSClient(queueSettings.IamAccessKey, queueSettings.IamSecretKey, RegionEndpoint.GetBySystemName(queueSettings.Region))
    );

builder.Services.AddSingleton<ISqsClientFactory, SqsClientFactory>();

builder.Services.AddHostedService<QueueConsumerService>();
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();


app.Run();
