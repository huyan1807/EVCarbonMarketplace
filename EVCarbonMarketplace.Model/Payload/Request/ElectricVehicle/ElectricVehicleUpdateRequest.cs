using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.ElectricVehicle
{
    public class ElectricVehicleUpdateRequest
    {
        public string? VehicleModel { get; set; }

        public string? Vin { get; set; }

        public decimal? BatteryCapacity { get; set; }

        public string? LicensePlate { get; set; }

        public string? Brand { get; set; }

        public Guid? VehicleTypeId { get; set; }

        public int? Odometer { get; set; }
    }
}
