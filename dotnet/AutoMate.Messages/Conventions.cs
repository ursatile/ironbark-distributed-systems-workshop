using System;
using AutoMate.Messages.Events;
using MassTransit;

namespace AutoMate.Messages
{
    public class Conventions
    {
        public static void MapEndpoints()
        {
            EndpointConvention.Map<NewVehicleListed>(new Uri("queue:new-vehicle-listed"));
        }
    }
}
