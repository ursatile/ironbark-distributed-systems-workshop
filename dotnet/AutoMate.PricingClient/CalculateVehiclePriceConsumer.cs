using System;
using System.Threading.Tasks;
using Autobarn.PricingEngine;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.PricingClient {
    public class CalculateVehiclePriceConsumer : IConsumer<CalculateVehiclePrice> {
        private readonly Pricer.PricerClient grpcClient;

        public CalculateVehiclePriceConsumer(Pricer.PricerClient grpcClient) {
            this.grpcClient = grpcClient;
        }
        public async Task Consume(ConsumeContext<CalculateVehiclePrice> context) {
            await Console.Out.WriteLineAsync(
                $"Using gRPC to get price for {context.Message.Manufacturer} {context.Message.VehicleModel}...");
            var request = new PriceRequest {
                Color = context.Message.Color,
                Year = context.Message.Year,
                Make = context.Message.Manufacturer,
                Model = context.Message.VehicleModel
            };
            var reply = await grpcClient.GetPriceAsync(request);
            await Console.Out.WriteLineAsync($"Got a price! {reply.Price} {reply.CurrencyCode}");
            await context.Publish<VehiclePriceCalculated>(new {
                context.Message.Registration,
                context.Message.CorrelationId,
                reply.Price,
                reply.CurrencyCode
            });
        }
    }
}