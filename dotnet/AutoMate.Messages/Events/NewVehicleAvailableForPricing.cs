using System;

namespace AutoMate.Messages.Events {
    public class NewVehicleAvailableForPricing {
        public string Manufacturer { get; set; }
        public string VehicleModel { get; set; }
        public string Registration { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public DateTimeOffset AvailabilityConfirmedAt { get; set; }
    }
}