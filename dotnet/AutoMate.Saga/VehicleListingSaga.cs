using System;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;
using Serilog;
using Serilog.Core;

namespace AutoMate.Saga {
    public class VehicleListingSaga : MassTransitStateMachine<VehicleListingState> {

        public Event<VehicleListingSubmitted> VehicleListingSubmitted { get; private set; }
        public Event<VehicleConfirmedStolen> VehicleConfirmedStolen { get; private set; }
        public Event<VehicleConfirmedWrittenOff> VehicleConfirmedWrittenOff { get; private set; }
        public Event<VehicleApprovedForListing> VehicleApprovedForListing { get; private set; }
        public Event<VehiclePriceCalculated> VehiclePriceCalculated { get; private set; }
        public Event<VehiclePutOnWebsite> VehiclePutOnWebsite { get; private set; }

        public State AwaitingStatus { get; private set; }
        public State AwaitingPrice { get; private set; }
        public State Stolen { get; private set; }
        public State WrittenOff { get; private set; }
        public State ReadyToPublish { get; private set; }

        public VehicleListingSaga() {

            InstanceState(x => x.CurrentState);

            Event(() => VehicleListingSubmitted, saga => saga.SelectId(_ => NewId.NextGuid()));
            Event(() => VehicleConfirmedWrittenOff);
            Event(() => VehicleConfirmedStolen);
            Event(() => VehicleApprovedForListing);
            Event(() => VehiclePriceCalculated);
            Event(() => VehiclePutOnWebsite);

            Initially(
                When(VehicleListingSubmitted)
                    .ThenAsync(async context => {
                        Log.Logger.Debug("Handling VehicleListingSubmitted!");
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:check-vehicle-status"));
                        context.Saga.Year = context.Message.Year;
                        context.Saga.Registration = context.Message.Registration;
                        context.Saga.Color = context.Message.Color;
                        context.Saga.Manufacturer = context.Message.Manufacturer;
                        context.Saga.VehicleModel = context.Message.VehicleModel;
                        await endpoint.Send<CheckVehicleStatus>(new {
                            context.Saga.CorrelationId,
                            context.Message.Registration,
                        });
                    }).TransitionTo(AwaitingStatus)
                );

            During(AwaitingStatus,
                When(VehicleConfirmedStolen)
                    .Then(context => {
                        Log.Logger.Warning($"VEHICLE REPORTED STOLEN! {context.Saga.Registration}");
                    })
                    .TransitionTo(Stolen),
                When(VehicleConfirmedWrittenOff)
                    .Then(context => {
                        Log.Logger.Warning("VEHICLE REPORTED WRITTEN OFF! {@message}", context.Message);
                    })
                    .TransitionTo(WrittenOff),
                When(VehicleApprovedForListing)
                    .ThenAsync(async context => {
                        Log.Logger.Information("Vehicle approved for listing {@message}", context.Message);
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:calculate-vehicle-price"));
                        await endpoint.Send<CalculateVehiclePrice>(new {
                            context.Saga.CorrelationId,
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
                    .ThenAsync(async context => {
                        Log.Logger.Information("Calculated a price: {@message}", context.Message);
                        context.Saga.Price = context.Message.Price;
                        context.Saga.CurrencyCode = context.Message.CurrencyCode;
                        var endpoint = await context.GetSendEndpoint(new Uri("queue:put-vehicle-on-website"));
                        await endpoint.Send<PutVehicleOnWebsite>(new {
                            context.Saga.CorrelationId,
                            context.Saga.Registration,
                            context.Saga.Year,
                            context.Saga.Color,
                            context.Saga.Manufacturer,
                            context.Saga.VehicleModel,
                            context.Saga.Price,
                            context.Saga.CurrencyCode
                        });
                    })
                    .TransitionTo(ReadyToPublish));
            During(ReadyToPublish,
                When(VehiclePutOnWebsite)
                    .Then(context => {
                        Console.WriteLine(
                            $"Vehicle with Rego: {context.Message.Registration} put on website");
                    })
                    .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}
