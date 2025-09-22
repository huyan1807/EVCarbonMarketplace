using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Wallet
{
    public class WalletResponse
    {
        public Guid Id { get; set; }

        public decimal? CarbonUnit { get; set; }

        public decimal? Cash { get; set; }


    }
}
