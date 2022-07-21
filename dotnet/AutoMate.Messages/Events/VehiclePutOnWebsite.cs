using System;
using MassTransit;

namespace AutoMate.Messages.Events {
    public interface VehiclePutOnWebsite : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }
}