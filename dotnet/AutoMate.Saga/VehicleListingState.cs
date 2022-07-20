using System;
using MassTransit;

namespace AutoMate.Saga {
    public class VehicleListingState : SagaStateMachineInstance {
        public Guid CorrelationId { get; set; }
        public string Registration { get; set; }
        public State CurrentState { get; set; }
    }
}