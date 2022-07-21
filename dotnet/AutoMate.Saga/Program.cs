using System;
using System.Threading.Tasks;
using AutoMate.Messages.Commands;
using AutoMate.Saga.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Graylog;

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
                            .RedisRepository();
                    });
                })
                .UseSerilog(ConfigureLogger)
                .Build();

            await host.StartAsync();
            var endpointProvider = host.Services.GetService<ISendEndpointProvider>();
            ISendEndpoint endpoint;
            var bus = host.Services.GetService<IBus>();
            Console.WriteLine(@"The saga is running!");
            while (true) {
                var registration = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 7);
                Console.WriteLine(String.Empty.PadRight(40, '='));
                Console.WriteLine("1: endpoint.Send<SubmitVehicleListing>()");
                Console.WriteLine("2: bus.Publish<SubmitVehicleListing>()");
                Console.WriteLine("3: endpoint.Send<CheckVehicleStatus>()");
                Console.WriteLine("4: bus.Publish<CheckVehicleStatus>()");
                switch (Console.ReadKey(true).Key) {
                    case ConsoleKey.Escape: goto ESCAPE;
                    case ConsoleKey.D1:
                        endpoint = await endpointProvider.GetSendEndpoint(new Uri("queue:submit-vehicle-listing"));
                        await endpoint.Send<SubmitVehicleListing>(new {
                            Color = "Silver",
                            Manufacturer = "DMC",
                            VehicleModel = "Delorean",
                            Registration = registration,
                            Year = 1985
                        });
                        Console.WriteLine($"Sent SubmitVehicleListing");
                        break;
                    case ConsoleKey.D2:
                        await bus.Publish<SubmitVehicleListing>(new { Registration = registration });
                        Console.WriteLine($"Published SubmitVehicleListing");
                        break;
                    case ConsoleKey.D3:
                        endpoint = await endpointProvider.GetSendEndpoint(new Uri("queue:check-vehicle-status"));
                        await endpoint.Send<CheckVehicleStatus>(new { Registration = registration });
                        Console.WriteLine($"Sent CheckVehicleStatus");
                        break;
                    case ConsoleKey.D4:
                        await bus.Publish<CheckVehicleStatus>(new { Registration = registration });
                        Console.WriteLine($"Published CheckVehicleStatus");
                        break;
                }
            }
        ESCAPE:
            Console.WriteLine("We're outta here!");
            await host.StopAsync();
        }

        private static void ConfigureLogger(HostBuilderContext host, LoggerConfiguration log) {
            log.MinimumLevel.Debug();
            log.WriteTo.Console();
            log.WriteTo.Graylog(new GraylogSinkOptions { HostnameOrAddress = "localhost", Port = 12201 });
            log.Enrich.WithProcessName();
        }
    }
}
