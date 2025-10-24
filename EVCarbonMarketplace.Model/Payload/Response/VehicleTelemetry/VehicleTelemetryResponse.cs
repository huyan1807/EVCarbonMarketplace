using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.VehicleTelemetry
{
    public class VehicleTelemetryResponse
    {
        public Guid Id { get; set; }
        public DateTime? LoggedAt { get; set; }
        public int? Odometer { get; set; }
        public decimal? DistanceTravelled { get; set; }
        public decimal? EnergyConsumed { get; set; }
        public decimal? BatteryLevel { get; set; }

        public string IsActive { get; set; }
    }
}
