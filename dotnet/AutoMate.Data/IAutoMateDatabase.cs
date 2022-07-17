using AutoMate.Data.Entities;
using System.Collections.Generic;

namespace AutoMate.Data {
    public interface IAutoMateDatabase {
        int CountVehicles();
        IEnumerable<Vehicle> ListVehicles();
        IEnumerable<Manufacturer> ListManufacturers();
        IEnumerable<VehicleModel> ListModels();

        Vehicle FindVehicle(string registration);
        VehicleModel FindModel(string code);
        Manufacturer FindManufacturer(string code);

        void CreateVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(Vehicle vehicle);
    }
}
