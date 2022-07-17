using AutoMate.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace AutoMate.Data {
    public class AutoMateCsvFileDatabase : IAutoMateDatabase {
        private static readonly IEqualityComparer<string> collation = StringComparer.OrdinalIgnoreCase;

        private readonly Dictionary<string, Manufacturer> manufacturers = new Dictionary<string, Manufacturer>(collation);
        private readonly Dictionary<string, VehicleModel> models = new Dictionary<string, VehicleModel>(collation);
        private readonly Dictionary<string, Vehicle> vehicles = new Dictionary<string, Vehicle>(collation);

        public AutoMateCsvFileDatabase() {
            ReadManufacturersFromCsvFile("manufacturers.csv");
            ReadModelsFromCsvFile("models.csv");
            ReadVehiclesFromCsvFile("vehicles.csv");
            ResolveReferences();
        }

        private void ResolveReferences() {
            foreach (var mfr in manufacturers.Values) {
                mfr.Models = models.Values.Where(m => m.ManufacturerCode == mfr.Code).ToList();
                foreach (var model in mfr.Models) model.Manufacturer = mfr;
            }

            foreach (var model in models.Values) {
                model.Vehicles = vehicles.Values.Where(v => v.ModelCode == model.Code).ToList();
                foreach (var vehicle in model.Vehicles) vehicle.VehicleModel = model;
            }
        }

        private string ResolveCsvFilePath(string filename, [CallerFilePath] string callerFilePath = "") {
            var dataDirectoryName = Path.GetDirectoryName(callerFilePath);
            var csvFileDirectoryName = Path.Combine(dataDirectoryName, "csv-data");
            return Path.Combine(csvFileDirectoryName, filename);
        }

        private void ReadVehiclesFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(',');
                var vehicle = new Vehicle {
                    Registration = tokens[0],
                    ModelCode = tokens[1],
                    Color = tokens[2]
                };
                if (Int32.TryParse(tokens[3], out var year)) vehicle.Year = year;
                vehicles[vehicle.Registration] = vehicle;
            }
        }

        private void ReadModelsFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(',');
                var model = new VehicleModel {
                    Code = tokens[0],
                    ManufacturerCode = tokens[1],
                    Name = tokens[2]
                };
                models.Add(model.Code, model);
            }
        }

        private void ReadManufacturersFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(',');
                var mfr = new Manufacturer {
                    Code = tokens[0],
                    Name = tokens[1]
                };
                manufacturers.Add(mfr.Code, mfr);
            }
        }

        public int CountVehicles() => vehicles.Count;

        public IEnumerable<Vehicle> ListVehicles() => vehicles.Values;

        public IEnumerable<Manufacturer> ListManufacturers() => manufacturers.Values;

        public IEnumerable<VehicleModel> ListModels() => models.Values;

        public Vehicle FindVehicle(string registration) =>
            vehicles.Values.FirstOrDefault(v => String.Equals(registration, v.Registration, StringComparison.InvariantCultureIgnoreCase));

        public VehicleModel FindModel(string code) => models.Values.FirstOrDefault(m => m.Code == code);

        public Manufacturer FindManufacturer(string code) => manufacturers.Values.FirstOrDefault(m => m.Code == code);

        public void CreateVehicle(Vehicle vehicle) {
            vehicle.VehicleModel.Vehicles.Add(vehicle);
            UpdateVehicle(vehicle);
        }

        public void UpdateVehicle(Vehicle vehicle) {
            vehicles[vehicle.Registration] = vehicle;
        }

        public void DeleteVehicle(Vehicle vehicle) {
            var model = FindModel(vehicle.ModelCode);
            model.Vehicles.Remove(vehicle);
            vehicles.Remove(vehicle.Registration);
        }
    }
}