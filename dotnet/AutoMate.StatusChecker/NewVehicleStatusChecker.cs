using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.StatusChecker {
    public static class VehicleStatus {
        public const string OK = nameof(OK);
        public const string WRITTEN_OFF = nameof(WRITTEN_OFF);
        public const string STOLEN = nameof(STOLEN);
    }

    public class CheckVehicleStatusConsumer : IConsumer<CheckVehicleStatus> {
        public const string STATUS_URI = "https://ursatile-vehicle-info-checker.azurewebsites.net/api/CheckVehicleStatus";

        public async Task Consume(ConsumeContext<CheckVehicleStatus> context) {
            var message = context.Message;
            var httpClient = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, GetStatusURL(message));
            var httpResponse = await httpClient.SendAsync(httpRequestMessage);
            var statusFromApi = await httpResponse.Content.ReadAsStringAsync();
            await Console.Out.WriteLineAsync($"Checking status for: {message.Registration}");
            await Console.Out.WriteLineAsync($"Status: {statusFromApi}");
            await Console.Out.WriteLineAsync($"context.CorrelationID: {context.CorrelationId}");
            await Console.Out.WriteLineAsync($"message.CorrelationID: {message.CorrelationId}");
            switch (statusFromApi) {
                case VehicleStatus.OK:
                    await context.Publish<VehicleApprovedForListing>(new { message.Registration, message.CorrelationId });
                    break;
                case VehicleStatus.STOLEN:
                    await context.Publish<VehicleConfirmedStolen>(new { message.Registration, message.CorrelationId });
                    break;
                case VehicleStatus.WRITTEN_OFF:
                    await context.Publish<VehicleConfirmedWrittenOff>(new { message.Registration, message.CorrelationId });
                    break;
            }
        }

        private static string GetStatusURL(CheckVehicleStatus message) {
            var builder = new UriBuilder(STATUS_URI) {
                Port = -1
            };
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["registration"] = message.Registration;
            builder.Query = query.ToString();
            var url = builder.ToString();
            return url;
        }
    }
}