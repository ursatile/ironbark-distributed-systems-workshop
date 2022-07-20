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

    public class VehicleListingSaga : MassTransitStateMachine<VehicleListingState> {

        public Event<VehicleListingSubmitted> VehicleListingSubmitted { get; private set; }

        public State AwaitingStatus { get; private set; }
        public State AwaitingPrice { get; private set; }
        public State Stolen { get; private set; }
        public State WrittenOff { get; private set; }

        public VehicleListingSaga() {

            InstanceState(x => x.CurrentState);

            Event(() => VehicleListingSubmitted, listing => listing
                .CorrelateBy(state => state.Registration, context => context.Message.Registration)
                .SelectId(context => Guid.NewGuid()));

            Initially(
                When(VehicleListingSubmitted)
                    .ThenAsync(async context => {
                        Console.WriteLine("Hey! We got a VehicleListingSubmitted event in our saga!");
                        Console.WriteLine($"{context.Message}");
                        await context.Publish<CheckVehicleStatus>(new {
                           registration = context.Message.Registration
                        });
                        //TODO: why does publishing work here but sending doesn't?
                        //var endpoint = await context.GetSendEndpoint(new Uri("queue:check-vehicle-status"));
                        //await endpoint.Send<CheckVehicleStatus>(new {
                        //    registration = context.Message.Registration
                        //});
                    }).TransitionTo(AwaitingStatus)
                );
        }
    }
}
