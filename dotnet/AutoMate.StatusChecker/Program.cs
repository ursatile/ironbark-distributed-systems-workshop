using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMate.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace AutoMate.StatusChecker {
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ReceiveEndpoint("dylanbeattie-automate-vehicle-status-checker", e => {
                                e.Consumer(() => new NewVehicleStatusChecker());
                            });
                        });
                    });
                })
                .Build().RunAsync();
            Console.WriteLine("AutoMate.AuditLog running! Press Ctrl-C to quit.");
        }
    }
    public class NewVehicleStatusChecker : IConsumer<NewVehicleListed> {
        private static readonly HttpClient httpClient = new HttpClient();
        public async Task Consume(ConsumeContext<NewVehicleListed> context) {
            var message = context.Message;
            var csv = $"{message.Registration},{message.Manufacturer},{message.VehicleModel},{message.Color},{message.Year},{message.ListedAt:O}";
            await Console.Out.WriteLineAsync(csv);
            var url =
                $"https://ursatile-vehicle-info-checker.azurewebsites.net/api/CheckVehicleStatus?registration={message.Registration}";
            var status = await httpClient.GetStringAsync(url);
            await Console.Out.WriteLineAsync($"Vehicle status: {status}");
            switch (status) {
                case "OK":
                    var vehicleReadyForPricing = new VehicleReadyForPricing {
                        VehicleModel = context.Message.VehicleModel,
                        Color = context.Message.Color,
                        Registration = context.Message.Registration,
                        Year = context.Message.Year
                    };
                    await context.Publish(vehicleReadyForPricing);
                    await Console.Out.WriteLineAsync("Published VehicleReadyForPricing!");
                    break;
                case "WRITTEN_OFF":
                case "STOLEN":
                    // TODO: what happens here?
                    break;
                default:
                    await Console.Out.WriteLineAsync("Invalid vehicle status. Oops.");
                    break;
            }
        }
    }
}