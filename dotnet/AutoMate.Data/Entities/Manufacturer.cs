using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoMate.Data.Entities {
    public partial class Manufacturer {
        public Manufacturer() {
            Models = new HashSet<VehicleModel>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<VehicleModel> Models { get; set; }
    }
}
