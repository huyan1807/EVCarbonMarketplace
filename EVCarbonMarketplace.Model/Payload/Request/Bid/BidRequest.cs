using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.Bid
{
    public class BidRequest
    {
        public Guid? CarbonListingId { get; set; }

        public decimal? Price { get; set; }

    }
}
