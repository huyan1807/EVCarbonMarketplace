using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle
{
    public class ElectricVehicleResponse
    {
        public Guid Id { get; set; }
        public string? VehicleModel { get; set; }

        public string? Vin { get; set; }

        public decimal? BatteryCapacity { get; set; }

        public string? LicensePlate { get; set; }

        public string? Brand { get; set; }

        public Guid? VehicleTypeId { get; set; }
        public string? VehicleType { get; set; }


        public int? Odometer { get; set; }

        public string ImageUrl { get; set; }
    }
}
