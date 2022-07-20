using System;

namespace AutoMate.Messages.Events {
    public interface VehicleListingSubmitted {
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Registration { get; set; }
        string Color { get; set; }
        int Year { get; set; }
        DateTimeOffset ListedAt { get; set; }
    }
}
