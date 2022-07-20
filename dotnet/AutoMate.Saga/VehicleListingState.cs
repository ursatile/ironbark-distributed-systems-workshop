using System;
using MassTransit;

namespace AutoMate.Saga {
    public class VehicleListingState : SagaStateMachineInstance {
        public Guid CorrelationId { get; set; }
        public string Registration { get; set; }
        public string Manufacturer { get; set; }
        public string VehicleModel { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Price { get; set; }
        public string CurrencyCode { get; set; }
        public State CurrentState { get; set; }

    }
}