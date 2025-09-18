using EVCarbonMarketplace.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonEmission
{
    public class CarbonEmissionResponse
    {
        public Guid Id { get; set; }

        public Guid? ElectricVehicleId { get; set; }

        public decimal? DistanceTravelled { get; set; }

        public decimal? EnergyConsumed { get; set; }

        public decimal? Co2reduced { get; set; }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }
        
        public string Status { get; set; }


    }
}
