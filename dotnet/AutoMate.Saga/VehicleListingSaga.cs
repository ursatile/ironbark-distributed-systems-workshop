using System;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;
using Serilog;

namespace AutoMate.Saga {
    public class VehicleListingSaga : MassTransitStateMachine<VehicleListingState> {

        public Event<VehicleListingSubmitted> VehicleListingSubmitted { get; private set; }
        public Event<VehicleConfirmedStolen> VehicleConfirmedStolen { get; private set; }
        public Event<VehicleConfirmedWrittenOff> VehicleConfirmedWrittenOff { get; private set; }
        public Event<VehicleApprovedForListing> VehicleApprovedForListing { get; private set; }
        public Event<VehiclePriceCalculated> VehiclePriceCalculated { get; private set; }

        public State AwaitingStatus { get; private set; }
        public State AwaitingPrice { get; private set; }
        public State Stolen { get; private set; }
        public State WrittenOff { get; private set; }
        public State ReadyToPublish { get; private set; }

        public VehicleListingSaga() {

            InstanceState(x => x.CurrentState);

            Event(() => VehicleListingSubmitted,
                saga => saga.CorrelateBy(state => state.Registration, context => context.Message.Registration)
                    .SelectId(_ => NewId.NextGuid()));
            ;
            Event(() => VehicleConfirmedWrittenOff,
                saga => saga.CorrelateBy(state => state.Registration, context => context.Message.Registration));
            Event(() => VehicleConfirmedStolen,
                saga => saga.CorrelateBy(state => state.Registration, context => context.Message.Registration));
            Event(() => VehicleApprovedForListing,
                saga => saga.CorrelateBy(state => state.Registration, context => context.Message.Registration));
            Event(() => VehiclePriceCalculated, 
                saga => saga.CorrelateBy(state => state.Registration, context => context.Message.Registration));

            Initially(
                When(VehicleListingSubmitted)
                    .ThenAsync(async context => {
                        Log.Logger.Debug("Received VehicleListingSubmitted {@message}", context.Message);
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:check-vehicle-status"));
                        context.Saga.Year = context.Message.Year;
                        context.Saga.Registration = context.Message.Registration;
                        context.Saga.Color = context.Message.Color;
                        context.Saga.Manufacturer = context.Message.Manufacturer;
                        context.Saga.VehicleModel = context.Message.VehicleModel;
                        await endpoint.Send<CheckVehicleStatus>(new {
                            context.Message.Registration,
                        });
                    }).TransitionTo(AwaitingStatus)
                );

            During(AwaitingStatus,
                When(VehicleConfirmedStolen)
                    .Then(context => {
                        Log.Logger.Warning("Vehicle Confirmed Stolen! {@message}", context.Message);
                        //var oldColor = Console.ForegroundColor;
                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.WriteLine($"VEHICLE REPORTED STOLEN! {context.Saga.Registration}");
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.ForegroundColor = oldColor;
                    })
                    .TransitionTo(Stolen),
                When(VehicleConfirmedWrittenOff)
                    .Then(context => {
                        Log.Logger.Warning("Vehicle Confirmed Written Off! {@message}", context.Message);
                        //var oldColor = Console.ForegroundColor;
                        //Console.ForegroundColor = ConsoleColor.Magenta;
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.WriteLine($"VEHICLE WRITTEN OFF! {context.Saga.Registration}");
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.ForegroundColor = oldColor;
                    })
                    .TransitionTo(WrittenOff),
                When(VehicleApprovedForListing)
                    .ThenAsync(async context => {
                        Log.Logger.Information("Vehicle approved for listing: {@message}", context.Message);
                        //var oldColor = Console.ForegroundColor;
                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.WriteLine($"Vehicle approved for listing. {context.Saga.Registration}");
                        //Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        //Console.ForegroundColor = oldColor;
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:calculate-vehicle-price"));
                        await endpoint.Send<CalculateVehiclePrice>(new {
                            context.Saga.Registration,
                            context.Saga.Year,
                            context.Saga.Color,
                            context.Saga.Manufacturer,
                            context.Saga.VehicleModel
                        });
                    })
                    .TransitionTo(AwaitingPrice));
            During(AwaitingPrice,
                When(VehiclePriceCalculated)
                    .Then(context => {
                        Console.WriteLine(
                            $"Calculated a price: {context.Message.Price} {context.Message.CurrencyCode}");
                        context.Saga.Price = context.Message.Price;
                        context.Saga.CurrencyCode = context.Message.CurrencyCode;
                    })
                    .TransitionTo(ReadyToPublish));
        }
    }
}
