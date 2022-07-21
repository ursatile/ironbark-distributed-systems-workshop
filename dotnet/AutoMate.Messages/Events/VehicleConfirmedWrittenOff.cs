using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehicleConfirmedWrittenOff : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }
}