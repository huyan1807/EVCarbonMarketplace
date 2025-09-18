using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonEmission
{
    public class CarbonEmissionDetailResponse
    {
        // Emission
        public Guid Id { get; set; }
        public string Status { get; set; }
        public decimal? DistanceTravelled { get; set; }
        public decimal? EnergyConsumed { get; set; }
        public decimal? Co2Reduced { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public DateTime? CreateAt { get; set; }

        // Owner
        public Guid? AccountId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerPhone { get; set; }

        // Vehicle
        public Guid? ElectricVehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public string? Vin { get; set; }
        public string? LicensePlate { get; set; }
        public string? Brand { get; set; }
        public decimal? BatteryCapacity { get; set; }
        public Guid? VehicleTypeId { get; set; }
        public string? VehicleTypeName { get; set; }
        public string? ImageUrl { get; set; }

    }
}
