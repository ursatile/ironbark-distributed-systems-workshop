using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehicleConfirmedStolen : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }
}