using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Graylog;

namespace AutoMate.StatusChecker {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.AddConsumersFromNamespaceContaining<CheckVehicleStatusConsumer>();
                        mt.SetKebabCaseEndpointNameFormatter();
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ConfigureEndpoints(context);
                        });
                    });
                })
                .UseSerilog(ConfigureLogger)
                .Build();
            await host.StartAsync();
            Console.WriteLine("AutoMate.StatusChecker running! Press Enter to quit.");
            Console.ReadLine();
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