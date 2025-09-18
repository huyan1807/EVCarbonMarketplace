using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonCredit
{
    public class CarbonCreditManageResponse
    {
        public Guid Id { get; set; }
        public Guid? CarbonEmissionId { get; set; }
        public Guid? AccountId { get; set; }

        public decimal? Credits { get; set; }
        public DateTime? CreateAt { get; set; }

        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }

        public string? OwnerPhone { get; set; }

        public Guid? ElectricVehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public string? LicensePlate { get; set; }
        public string? Brand { get; set; }

        public string? VehicleType { get; set; }

        public string? ImageUrl { get; set; }
    }
}
