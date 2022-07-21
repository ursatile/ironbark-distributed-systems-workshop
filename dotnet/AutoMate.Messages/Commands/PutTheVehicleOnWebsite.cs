using System;
using MassTransit;

namespace AutoMate.Messages.Commands
{
    public interface PutVehicleOnWebsite : CorrelatedBy<Guid>
    {
        string Registration { get; set; }
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Color { get; set; }
        int Year { get; set; }
        int Price { get; set; }
        string CurrencyCode { get; set; }
    }
}