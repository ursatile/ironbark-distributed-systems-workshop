using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.StatusChecker {
    public class NewVehicleStatusChecker : IConsumer<VehicleListingSubmitted> {
        public const string STATUS_URI = "https://ursatile-vehicle-info-checker.azurewebsites.net/api/CheckVehicleStatus";
        public const string STATUS_OK = "OK";
        public async Task Consume(ConsumeContext<VehicleListingSubmitted> context) {
            var message = context.Message;
            var csv = $"{message.Registration},{message.Manufacturer},{message.VehicleModel},{message.Color},{message.Year},{message.ListedAt:O}";


            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, GetStatusURL(message));
            var httpResponse = await httpClient.SendAsync(httpRequestMessage);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();
            await Console.Out.WriteLineAsync(csv);
            await Console.Out.WriteLineAsync($"Status {httpContent}");
            if (httpContent == STATUS_OK) {
                await context.Publish<NewVehicleAvailableForPricing>(new NewVehicleAvailableForPricing {
                    AvailabilityConfirmedAt = DateTimeOffset.UtcNow,
                    Color = message.Color,
                    Manufacturer = message.Manufacturer,
                    Registration = message.Registration,
                    VehicleModel = message.VehicleModel,
                    Year = message.Year,
                });
            }
        }

        private static string GetStatusURL(VehicleListingSubmitted message) {
            var builder = new UriBuilder(STATUS_URI);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["registration"] = message.Registration;
            builder.Query = query.ToString();
            string url = builder.ToString();
            return url;
        }
    }
}