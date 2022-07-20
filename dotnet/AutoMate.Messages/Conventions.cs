using System;
using AutoMate.Messages.Commands;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Messages {
    public class Conventions {
        public static void MapEndpoints() {
            EndpointConvention.Map<VehicleListingSubmitted>(new Uri("queue:vehicle-listing-submitted"));
        }
    }
}
