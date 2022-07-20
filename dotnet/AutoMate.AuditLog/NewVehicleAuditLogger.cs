using System;
using System.Threading.Tasks;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.AuditLog {
    public class NewVehicleAuditLogger : IConsumer<VehicleListingSubmitted> {
        public async Task Consume(ConsumeContext<VehicleListingSubmitted> context) {
            var message = context.Message;
            var csv = $"{message.Registration},{message.Manufacturer},{message.VehicleModel},{message.Color},{message.Year},{message.ListedAt:O}";
            await Console.Out.WriteLineAsync(csv);
        }
    }
}