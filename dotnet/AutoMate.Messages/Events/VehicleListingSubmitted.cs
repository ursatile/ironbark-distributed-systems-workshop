using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehicleListingSubmitted : CorrelatedBy<Guid> {
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Registration { get; set; }
        string Color { get; set; }
        int Year { get; set; }
        DateTimeOffset ListedAt { get; set; }
    }

    public interface VehicleConfirmedStolen : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }

    public interface VehicleConfirmedWrittenOff : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }

    public interface VehicleApprovedForListing : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }

    public interface VehiclePriceCalculated : CorrelatedBy<Guid> {
        int Price { get; set; }
        string CurrencyCode { get; set; }
    }
}
