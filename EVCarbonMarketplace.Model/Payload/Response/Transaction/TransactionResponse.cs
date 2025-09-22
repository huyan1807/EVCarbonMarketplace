using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Transaction
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Credits { get; set; }
        public DateTime CreateAt { get; set; }

        public string Type { get; set; }
        public decimal? Price { get; set; }

        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAvatar { get; set; }

        public Guid SellerId { get; set; }
        public string SellerName { get; set; }
        public string SellerAvatar { get; set; }

        public Guid CarbonCreditId { get; set; }
        public DateTime EmissionStart { get; set; }
        public DateTime EmissionEnd { get; set; }
        public decimal Co2Reduced { get; set; }
    }
}
