using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Analytic
{
    public class TransactionStatsResponse
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int SoldListings { get; set; }
        public int ExpiredListings { get; set; }
        public int CancelledListings { get; set; }

        public int TotalTransactions { get; set; }
        public int FixedPriceTransactions { get; set; }
        public int AuctionTransactions { get; set; }

        public decimal TotalCreditsSold { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        public List<TransactionDailyStats> ByDate { get; set; }
    }
}
