using System;
using System.Threading.Tasks;
using AutoMate.Notifier;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoMate.Notifier {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {

            var signalRHubUrl = "https://workshop.ursatile.com:5001/hub";
            var hub = new HubConnectionBuilder().WithUrl(signalRHubUrl).Build();
            await hub.StartAsync();
            Console.WriteLine($"Using SignalR hub at {signalRHubUrl}");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddSingleton(hub);
                    services.AddMassTransit(mt => {
                        mt.AddConsumersFromNamespaceContaining<PutVehicleOnWebsiteConsumer>();
                        mt.SetKebabCaseEndpointNameFormatter();
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ConfigureEndpoints(context);
                        });
                    });
                })
                .Build();
            await host.StartAsync();
            Console.WriteLine("AutoMate.VehicleNotifer running! Press Enter to quit.");
            Console.ReadLine();
            await hub.StopAsync();
            await host.StopAsync();
        }
    }
}
