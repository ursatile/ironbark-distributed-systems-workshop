using System;

namespace AutoMate.Messages.Commands {
    public interface CheckVehicleStatus {
        string Registration { get; set; }
    }

    public interface SubmitVehicleListing {
        string Registration { get; set; }
        string Manufacturer { get; set; }
        string VehicleModel { get; set; }
        string Color { get; set; }
        int Year { get; set; }
        DateTimeOffset VehicleListedAt { get; set; }
    }

    //public class SubmitVehicleListing {
    //    public override string ToString() {
    //        return $"{Registration} ({Manufacturer} {VehicleModel}, {Color}, {Year})";
    //    }

    //    public string Registration { get; set; }
    //    public string Manufacturer { get; set; }
    //    public string VehicleModel { get; set; }
    //    public string Color { get; set; }
    //    public int Year { get; set; }
    //    public DateTimeOffset VehicleListedAt { get; set; }
    //}

}