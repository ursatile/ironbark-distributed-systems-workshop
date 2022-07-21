using System;
using System.Threading.Tasks;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Saga.Consumers {
    public class SubmitVehicleListingConsumer : IConsumer<SubmitVehicleListing> {
        public async Task Consume(ConsumeContext<SubmitVehicleListing> context) {
            await Console.Out.WriteLineAsync($"We got a SubmitVehicleListing command: {context.Message.Registration}");
            var evt = new {
                context.Message.CorrelationId,
                context.Message.Color,
                ListedAt = DateTimeOffset.UtcNow,
                context.Message.Year,
                context.Message.Manufacturer,
                context.Message.Registration,
                context.Message.VehicleModel
            };
            await context.Publish<VehicleListingSubmitted>(evt);
            await Console.Out.WriteLineAsync($"Published event.");
        }
    }
}
