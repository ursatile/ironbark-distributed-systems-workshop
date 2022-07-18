using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace AutoMate.AuditLog
{
    class Program {
        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddMassTransit(mt => {
                        mt.UsingRabbitMq((context, config) => {
                            config.Host(RABBITMQ_URL);
                            config.ReceiveEndpoint("new-vehicle-listed", e => {
                                e.Consumer(() => new NewVehicleAuditLogger());
                            });
                        });
                    });
                })
                .Build().RunAsync();
            Console.WriteLine("AutoMate.AuditLog running! Press Ctrl-C to quit.");
        }
    }
}
