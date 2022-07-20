using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Saga.Consumers {
    public class SubmitVehicleListingConsumer : IConsumer<SubmitVehicleListing> {
        public async Task Consume(ConsumeContext<SubmitVehicleListing> context) {
            await Console.Out.WriteLineAsync($"We got a SubmitVehicleListing command: {context.Message.Registration}");
            var evt = new VehicleListingSubmitted
            {
                Color = context.Message.Color,
                ListedAt = DateTimeOffset.UtcNow,
                Year = context.Message.Year,
                Manufacturer = context.Message.Manufacturer,
                Registration = context.Message.Registration,
                VehicleModel = context.Message.VehicleModel
            };
            await context.Publish(evt);
            await Console.Out.WriteLineAsync($"Published event.");
        }
    }
}
