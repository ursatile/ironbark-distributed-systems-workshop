using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using AutoMate.Saga.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoMate.Saga {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.AddConsumersFromNamespaceContaining<SubmitVehicleListingConsumer>();
                        mt.SetKebabCaseEndpointNameFormatter();
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ConfigureEndpoints(context);
                        });
                        mt.AddSagaStateMachine<VehicleListingSaga, VehicleListingState>()
                            .InMemoryRepository();
                    });
                })
                .Build();

            await host.StartAsync();
            var endpointProvider = host.Services.GetService<ISendEndpointProvider>();
            var endpoint = await endpointProvider.GetSendEndpoint(new Uri("queue:submit-vehicle-listing"));
            Console.WriteLine("The saga is running! Press any key to send a SubmitVehicleListing command...");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) {
                var registration = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 7);
                var command = new {
                    Color = "Silver",
                    Manufacturer = "DMC",
                    VehicleModel = "Delorean",
                    Registration = registration,
                    Year = 1985
                };
                await endpoint.Send<SubmitVehicleListing>(command);
                Console.WriteLine($"Sent command: {command}");
            }
            await host.StopAsync();
        }
    }
}
