using System;
using MassTransit;

namespace AutoMate.Messages.Commands {
    public interface CheckVehicleStatus : CorrelatedBy<Guid> {
        string Registration { get; set; }
    }
}