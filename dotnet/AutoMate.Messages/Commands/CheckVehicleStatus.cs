using System;
using MassTransit;

namespace AutoMate.Messages.Commands {
    public interface CheckVehicleStatus : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }

    public interface SubmitVehicleListing: CorrelatedBy<Guid> {
        string Registration { get; set; }
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Color { get; set; }
        int Year { get; set; }
        DateTimeOffset VehicleListedAt { get; set; }
    }

    public interface CalculateVehiclePrice : CorrelatedBy<Guid> {
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Color { get; set; }
        int Year { get; set; }
    }
}