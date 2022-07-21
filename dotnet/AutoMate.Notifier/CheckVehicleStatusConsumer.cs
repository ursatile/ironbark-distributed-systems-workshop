using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace AutoMate.Notifier {
    public class PutVehicleOnWebsiteConsumer : IConsumer<PutVehicleOnWebsite> {
        private readonly HubConnection hub;

        public PutVehicleOnWebsiteConsumer(HubConnection hub) {
            this.hub = hub;
        }

        public async Task Consume(ConsumeContext<PutVehicleOnWebsite> context) {
            var message = context.Message;
            var json = JsonConvert.SerializeObject(message);
            await Console.Out.WriteLineAsync($"Putting it on the website {message.Registration}");
            await hub.SendAsync("NotifyWebUsers", "AutoMate.Notifier", json);
            await context.Publish<VehiclePutOnWebsite>(new { message.Registration, message.CorrelationId });
        }

    }
}