using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.TelemetryRecord
{
    public class TelemetryRecordResponse
    {
        public DateTime LoggedAt { get; set; }
        public decimal Odometer { get; set; }
        public decimal DistanceTravelled { get; set; }
        public decimal EnergyConsumed { get; set; }
        public decimal BatteryLevel { get; set; }
    }
}
