using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonCredit
{
    public class CarbonCreditResponse
    {
        public Guid Id { get; set; }
        public Guid? CarbonEmissionId { get; set; }
        public decimal? Credits { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? ElectricVehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public string? LicensePlate { get; set; }
        public string? Brand { get; set; }
        public decimal? BatteryCapacity { get; set; }

        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public decimal? Co2Reduced { get; set; }
    }
}
