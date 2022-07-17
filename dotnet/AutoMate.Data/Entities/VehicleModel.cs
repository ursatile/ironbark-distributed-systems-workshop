using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoMate.Data.Entities {
    public sealed partial class VehicleModel {
        public VehicleModel() {
            Vehicles = new HashSet<Vehicle>();
        }

        public string Code { get; set; }
        public string ManufacturerCode { get; set; }
        public string Name { get; set; }

        public Manufacturer Manufacturer { get; set; }
        
        [JsonIgnore]
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
