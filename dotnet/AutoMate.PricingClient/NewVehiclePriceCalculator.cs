using System;
using System.Threading.Tasks;
using Autobarn.PricingEngine;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;
using Serilog;
using Serilog.Core;

namespace AutoMate.PricingClient {
    public class NewVehiclePriceCalculator : IConsumer<CalculateVehiclePrice> {
        private readonly Pricer.PricerClient grpcClient;

        public NewVehiclePriceCalculator(Pricer.PricerClient grpcClient) {
            this.grpcClient = grpcClient;
        }
        public async Task Consume(ConsumeContext<CalculateVehiclePrice> context) {
            Log.Logger.Debug("using gRPC to get price for {@message}", context.Message);
            var request = new PriceRequest {
                Color = context.Message.Color,
                Year = context.Message.Year,
                Make = context.Message.Manufacturer,
                Model = context.Message.VehicleModel
            };
            var reply = await grpcClient.GetPriceAsync(request);
            Log.Logger.Debug("Received reply from gRPC server: {@reply}", reply);
            await context.Publish<VehiclePriceCalculated>(new {
                context.Message.Registration,
                reply.Price,
                reply.CurrencyCode
            });
        }
    }
}