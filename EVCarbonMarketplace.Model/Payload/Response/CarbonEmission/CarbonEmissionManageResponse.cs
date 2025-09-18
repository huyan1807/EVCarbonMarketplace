using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonEmission
{
    public class CarbonEmissionManageResponse
    {
        public Guid Id { get; set; }

        public Guid? ElectricVehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public string? LicensePlate { get; set; }

        public Guid? AccountId { get; set; }

        public string? OwnerName { get; set; }

        public decimal? DistanceTravelled { get; set; }

        public decimal? EnergyConsumed { get; set; }

        public decimal? Co2reduced { get; set; }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }

        public string Status { get; set; }
        public DateTime? CreateAt { get; set; }


    }
}
