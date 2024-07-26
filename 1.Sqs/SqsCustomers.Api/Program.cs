using Amazon;
using Amazon.SQS;
using Customers.Api.Database;
using Customers.Api.Messaging;
using Customers.Api.Repositories;
using Customers.Api.Services;
using Customers.Api.Validation;
using Dapper;
using FluentValidation.AspNetCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

var config = builder.Configuration;
config.AddEnvironmentVariables("CustomersApi_");

builder.Services.AddControllers().AddFluentValidation(x =>
{
    x.RegisterValidatorsFromAssemblyContaining<Program>();
    x.DisableDataAnnotationsValidation = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "A set of EndPoints using AWS SQS.", Version = "v1" });
});

SqlMapper.AddTypeHandler(new GuidTypeHandler());
SqlMapper.RemoveTypeMap(typeof(Guid));
SqlMapper.RemoveTypeMap(typeof(Guid?));

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(config.GetValue<string>("Database:ConnectionString")!));
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
var queueSettings = new QueueSettings();
builder.Configuration.GetRequiredSection(QueueSettings.Key).Bind(queueSettings);

builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>(_ =>
    new AmazonSQSClient(queueSettings.IamAccessKey, queueSettings.IamSecretKey, RegionEndpoint.GetBySystemName(queueSettings.Region))
    );

builder.Services.AddSingleton<ISqsMessengerService, SqsMessengerService>();
builder.Services.AddSingleton<ISqsClientFactory, SqsClientFactory>();

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IGitHubService, GitHubService>();

builder.Services.AddHttpClient("GitHub", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("GitHub:ApiBaseUrl")!);
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/vnd.github.v3+json");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.MapControllers();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();
