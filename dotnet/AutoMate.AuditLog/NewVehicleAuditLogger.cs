using System;
using System.Threading.Tasks;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.AuditLog {
    public class NewVehicleAuditLogger : IConsumer<NewVehicleListed> {
        public async Task Consume(ConsumeContext<NewVehicleListed> context) {
            var message = context.Message;
            var csv = $"{message.Registration},{message.Manufacturer},{message.VehicleModel},{message.Color},{message.Year},{message.ListedAt:O}";
            await Console.Out.WriteLineAsync(csv);
        }
    }
}