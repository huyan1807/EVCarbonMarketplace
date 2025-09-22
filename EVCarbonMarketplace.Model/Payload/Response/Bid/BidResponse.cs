using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Bid
{
    public class BidResponse
    {

        public Guid Id { get; set; }

        public Guid? CarbonListingId { get; set; }

        public Guid? AccountId { get; set; }

        public DateTime? BidTime { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateAt { get; set; }

    }
}
