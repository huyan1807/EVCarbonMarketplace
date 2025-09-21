using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.CarbonListing
{
    public class CarbonListingResponse 
    {
        public Guid Id { get; set; }
        public Guid CarbonCreditId { get; set; }
        public Guid SellerId { get; set; }
        public decimal? Price { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
