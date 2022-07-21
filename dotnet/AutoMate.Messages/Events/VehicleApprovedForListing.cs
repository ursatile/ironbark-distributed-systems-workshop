using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehicleApprovedForListing : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }
}