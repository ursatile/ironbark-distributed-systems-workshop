using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Autobarn.PricingEngine;
using Serilog;
using Serilog.Sinks.Graylog;

namespace AutoMate.PricingClient {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";

        private const string GRPC_URL = "https://workshop.ursatile.com:5003/";

        static async Task Main(string[] args) {
            using var channel = GrpcChannel.ForAddress(GRPC_URL);
            var grpcClient = new Pricer.PricerClient(channel);
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ConfigureEndpoints(context);
                            config.ReceiveEndpoint("calculate-vehicle-price", e => {
                                e.Consumer(() => new CalculateVehiclePriceConsumer(grpcClient));
                                //Message Retry Here.
                            });
                        });
                    });
                })
                .UseSerilog(ConfigureLogger)
                .Build().RunAsync();
            Console.WriteLine("AutoMate.PricingClient is running! Press Ctrl-C to quit.");
        }
        private static void ConfigureLogger(HostBuilderContext host, LoggerConfiguration log) {
            log.MinimumLevel.Debug();
            log.WriteTo.Console();
            log.WriteTo.Graylog(new GraylogSinkOptions { HostnameOrAddress = "localhost", Port = 12201 });
            log.Enrich.WithProcessName();
        }
    }
}