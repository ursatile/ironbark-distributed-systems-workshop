using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehiclePriceCalculated : CorrelatedBy<Guid> {
        string Registration { get; set; }
        int Price { get; set; }
        string CurrencyCode { get; set; }
    }
}