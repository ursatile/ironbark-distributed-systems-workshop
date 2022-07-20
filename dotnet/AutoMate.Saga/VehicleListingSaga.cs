using System;
using System.Collections.Generic;
using System.Text;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Saga {
    public class VehicleListingSaga : MassTransitStateMachine<VehicleListingState> {

        public Event<VehicleListingSubmitted> VehicleListingSubmitted { get; private set; }
        public Event<VehicleConfirmedStolen> VehicleConfirmedStolen { get; private set; }
        public Event<VehicleConfirmedWrittenOff> VehicleConfirmedWrittenOff { get; private set; }
        public Event<VehicleApprovedForListing> VehicleApprovedForListing { get; private set; }

        public State AwaitingStatus { get; private set; }
        public State AwaitingPrice { get; private set; }
        public State Stolen { get; private set; }
        public State WrittenOff { get; private set; }

        public VehicleListingSaga() {

            InstanceState(x => x.CurrentState);

            Event(() => VehicleListingSubmitted, listing => listing.CorrelateBy(state => state.Registration, context => context.Message.Registration)
                .SelectId(context => Guid.NewGuid()));

            Event(() => VehicleConfirmedWrittenOff, listing => listing.CorrelateBy(state => state.Registration, context => context.Message.Registration));
            Event(() => VehicleConfirmedStolen, listing => listing.CorrelateBy(state => state.Registration, context => context.Message.Registration));
            Event(() => VehicleApprovedForListing, listing => listing.CorrelateBy(state => state.Registration, context => context.Message.Registration));

            Initially(
                When(VehicleListingSubmitted)
                    .ThenAsync(async context => {
                        Console.WriteLine("Hey! We got a VehicleListingSubmitted event in our saga!");
                        Console.WriteLine($"{context.Message}");
                        //await context.Publish<CheckVehicleStatus>(new {
                        //    registration = context.Message.Registration
                        //});
                        //TODO: why does publishing work here but sending doesn't?
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:check-vehicle-status"));
                        await endpoint.Send<CheckVehicleStatus>(new {
                            registration = context.Message.Registration
                        });
                    }).TransitionTo(AwaitingStatus)
                );
            During(AwaitingStatus,
                When(VehicleConfirmedStolen)
                    .Then(context => {
                        var oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.WriteLine($"VEHICLE REPORTED STOLEN! {context.Saga.Registration}");
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.ForegroundColor = oldColor;
                    })
                    .TransitionTo(Stolen),
                When(VehicleConfirmedWrittenOff)
                    .Then(context => {
                        var oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.WriteLine($"VEHICLE REPORTED STOLEN! {context.Saga.Registration}");
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.ForegroundColor = oldColor;
                    })
                    .TransitionTo(WrittenOff),
                When(VehicleApprovedForListing)
                    .Then(context => {
                        var oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.WriteLine($"VEHICLE REPORTED STOLEN! {context.Saga.Registration}");
                        Console.WriteLine(String.Empty.PadRight(60, 'X'));
                        Console.ForegroundColor = oldColor;
                    })
                    .TransitionTo(AwaitingPrice));


        }
    }
}
