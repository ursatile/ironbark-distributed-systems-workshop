using System;
using System.Threading.Tasks;
using AutoMate.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace AutoMate.PricingClient {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ReceiveEndpoint("automate-pricing-client", e => {
                                e.Consumer(() => new NewVehiclePriceCalculator());
                            });
                        });
                    });
                })
                .Build().RunAsync();
            Console.WriteLine("AutoMate.PricingClient is running! Press Ctrl-C to quit.");
        }
    }

    public class NewVehiclePriceCalculator : IConsumer<NewVehicleAvailableForPricing> {
        public async Task Consume(ConsumeContext<NewVehicleAvailableForPricing> context) {
            await Console.Out.WriteLineAsync(context.Message.Registration);
        }
    }
}