using System;
using System.Threading.Tasks;
using AutoMate.Messages.Events;
using Grpc.Net.Client;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Autobarn.PricingEngine;

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
                            config.ReceiveEndpoint("automate-pricing-client", e => {
                                e.Consumer(() => new NewVehiclePriceCalculator(grpcClient));
                            });
                        });
                    });
                })
                .Build().RunAsync();
            Console.WriteLine("AutoMate.PricingClient is running! Press Ctrl-C to quit.");
        }
    }

    public class NewVehiclePriceCalculator : IConsumer<NewVehicleAvailableForPricing> {
        private readonly Pricer.PricerClient grpcClient;

        public NewVehiclePriceCalculator(Pricer.PricerClient grpcClient) {
            this.grpcClient = grpcClient;
        }
        public async Task Consume(ConsumeContext<NewVehicleAvailableForPricing> context) {
            await Console.Out.WriteLineAsync(
                $"Using gRPC to get price for {context.Message.Manufacturer} {context.Message.VehicleModel}...");
            var request = new PriceRequest {
                Color = context.Message.Color,
                Year = context.Message.Year,
                Make = context.Message.Manufacturer,
                Model = context.Message.VehicleModel
            };
            var reply = await grpcClient.GetPriceAsync(request);
            await Console.Out.WriteLineAsync($"Got a price! {reply.Price} {reply.CurrencyCode}");
            // TODO: publish something here?
        }
    }
}