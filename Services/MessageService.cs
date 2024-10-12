using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
public class MessageService : IHostedService
{

    // Två variabler för att spara kopplingen till RabbitMQ
    private IConnection connection;
    private IModel channel;

    // Anslut till RabbitMQ
    public void Connect()
    {
        var factory = new ConnectionFactory { HostName = "10.104.114.61", Port = 5672 };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        
        channel.ExchangeDeclare("delete-listing", ExchangeType.Fanout);
        
        channel.ExchangeDeclare("logging", ExchangeType.Fanout);

    }

    // Skicka ett meddelande om borttagningen av en annons
    public void NotifyListingDelete(string id)
    {
        var message = Encoding.UTF8.GetBytes(id);

        channel.BasicPublish("delete-listing", string.Empty, null, message);
    }

    public void SendLoggingActions(string action)
    {
        var message = Encoding.UTF8.GetBytes(action);
        channel.BasicPublish("logging", string.Empty, null, message);
    }


    // Anropas när programmet startas
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();
        return Task.CompletedTask;
    }

    // Anropas när programmet stoppas, och då kopplar vi bort från
    // RabbitMQ
    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }
}