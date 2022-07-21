using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Notifier {
    public class PutVehicleOnWebsiteConsumer : IConsumer<PutVehicleOnWebsite> {
        public async Task Consume(ConsumeContext<PutVehicleOnWebsite> context) {
            var message = context.Message;

            await Console.Out.WriteLineAsync($"Putting it on the website {message.Registration}");

            await context.Publish<VehiclePutOnWebsite>(new { message.Registration, message.CorrelationId });
        }

    }
}