using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Enum
{
    public class CarbonListingEnum
    {
        public enum ListingType
        {
            FixedPrice,
            Auction
        }

        public enum ListingStatus
        {
            Active,
            Sold,
            Expired,
            Cancelled
        }
    }
}
