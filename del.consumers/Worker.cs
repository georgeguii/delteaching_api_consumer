using del.shared.DTOs;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace del.consumers;

public class Worker : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private readonly string _mainQueueName = "george_queues.in";
    private readonly string _retryQueueName = "george_queues.wait";
    private readonly string _errorQueueName = "george_queues.error";

    public Worker(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMqAsync();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await ProcessMessageAsync(ea, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erro: {ex.Message}");
                await RetryOrMoveToErrorQueue<BankAccountDTO>(ea);
            }
        };

        await _channel.BasicConsumeAsync(_mainQueueName, autoAck: true, consumer: consumer);
    }

    private async Task InitializeRabbitMqAsync()
    {
        var rabbitMqConnectionString = _configuration["ConnectionStrings:RabbitMq"];

        if (string.IsNullOrEmpty(rabbitMqConnectionString) || !Uri.TryCreate(rabbitMqConnectionString, UriKind.Absolute, out Uri? uri) || uri.Scheme != "amqp")
        {
            throw new InvalidOperationException("A string de conexão do RabbitMQ não é válida ou está mal formatada.");
        }

        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var username = userInfo[0];
        var password = userInfo[1];

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await DeclareQueuesAsync();
    }

    private async Task DeclareQueuesAsync()
    {
        await _channel.QueueDeclareAsync(
            queue: _errorQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var retryArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _mainQueueName },
            { "x-message-ttl", 5000 }
        };

        await _channel.QueueDeclareAsync(
            queue: _retryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArguments);

        var mainArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _errorQueueName }
        };

        await _channel.QueueDeclareAsync(
            queue: _mainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainArguments);
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, string message)
    {
        var wrapper = JsonSerializer.Deserialize<MessageWrapper<BankAccountDTO>>(message);
        if (wrapper == null)
        {
            Console.WriteLine("Mensagem não está no formato esperado.");
            throw new InvalidCastException("Mensagem não está no formato esperado.");
        }

        var url = "http://localhost:5022/api/v1/bank-accounts";
        var content = new StringContent(JsonSerializer.Serialize(wrapper.Message), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API: {response.StatusCode}, {responseContent}");
        }

        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }

    private async Task RetryOrMoveToErrorQueue<T>(BasicDeliverEventArgs ea)
    {
        const int maxRetryCount = 3;

        var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
        var wrapper = JsonSerializer.Deserialize<MessageWrapper<T>>(messageBody);

        if (wrapper == null)
        {
            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            return;
        }

        if (wrapper.RetryCount < maxRetryCount)
        {
            wrapper.RetryCount++;
            var retryMessageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wrapper));

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _retryQueueName,
                body: retryMessageBody
            );
        }
        else
        {
            var errorMessageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wrapper));
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _errorQueueName,
                body: errorMessageBody
            );
        }

        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }
}
