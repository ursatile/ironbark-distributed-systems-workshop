# Exercise: Building a microservice

In this exercise, you're going to create the microservice component called `AutoMate.StatusChecker` in the following diagram:

![image-20220719064824676](image-20220719064824676.png)

You'll need to:

Create a new .NET Console application (targetting .NET Core 3.1, 5, or 6)

Created a hosted application using the following code structure in your main program class:

```csharp
class Program {
    private const string RABBITMQ_URL =
        "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";

    private const string QUEUE_NAME = ""; // add a unique queue name here

    static async Task Main(string[] args) {
    	await Host.CreateDefaultBuilder(args)
		    .ConfigureServices(services => {
			    services.AddMassTransit(mt => {
				    mt.UsingRabbitMq((context, config) => {
				    config.Host(RABBITMQ_URL);
				    config.ReceiveEndpoint(QUEUE_NAME, e => {
					    e.Consumer(() => new NewVehicleStatusChecker());
				    });
			    });
		    });
	    })
    .Build().RunAsync();
    Console.WriteLine("AutoMate.AuditLog running! Press Ctrl-C to quit.");
  }
}
```

This will subscribe to messages on a queue, and register a `NewVehicleStatusChecker` to handle incoming messages.

Create the `NewVehicleStatusChecker`service based on this code:

```csharp
public class NewVehicleAuditLogger : IConsumer<NewVehicleListed> {
    public async Task Consume(ConsumeContext<NewVehicleListed> context) {
        var message = context.Message;
        var csv = $"{message.Registration},{message.Manufacturer},{message.VehicleModel},{message.Color},{message.Year},{message.ListedAt:O}";
        await Console.Out.WriteLineAsync(csv);
        
        // TODO: Create an HTTP request
        // Check the vehicle status against the Azure function below
        // If the status is OK, publish a NewVehicleAvailableForPricing event
    }
}
```

The Vehicle Info Endpoint

Send an HTTP GET request to:

https://ursatile-vehicle-info-checker.azurewebsites.net/api/CheckVehicleStatus?registration={registration}

The API will return an HTTP response containing the vehicle status as plain text, which will be one of

* `OK` - the vehicle is OK to list for sale
* `STOLEN` - the vehicle is reported stolen
* `WRITTEN_OFF` - the vehicle has been reported as an insurance write-off
* `INVALID` - you did not provide a valid vehicle registration

