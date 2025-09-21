using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.CarbonListing
{
    public class CarbonListingUpdateRequest
    {
        public decimal? Price { get; set; }      
        public DateTime? EndTime { get; set; }  
    }
}
