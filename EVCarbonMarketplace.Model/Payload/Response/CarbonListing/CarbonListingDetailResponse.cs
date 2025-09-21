using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonListing
{
    public class CarbonListingDetailResponse
    {
        public Guid Id { get; set; }

        public decimal Credits { get; set; }

        public decimal? Price { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public Guid CarbonCreditId { get; set; }

        public DateTime EmissionStart { get; set; }
        public DateTime EmissionEnd { get; set; }
        public decimal DistanceTravelled { get; set; }
        public decimal EnergyConsumed { get; set; }
        public decimal Co2Reduced { get; set; }

        public Guid SellerId { get; set; }
        public string SellerName { get; set; }
        public string SellerAvatar { get; set; }

        public Guid VehicleId { get; set; }
        public string VehicleModel { get; set; }
        public string Brand { get; set; }
        public string VehicleType { get; set; }
        public decimal? BatteryCapacity { get; set; }
        public int? Odometer { get; set; }
        public string VehicleImage { get; set; }
    }
}
