using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Dispute
{
    public class DisputeDetailResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
        public string? EvidenceUrl { get; set; }
        public DateTime? CreateAt { get; set; }

        public Guid? SendAccountId { get; set; }
        public string? SendAccountName { get; set; }

        public Guid? TransactionId { get; set; }
        public string? TransactionType { get; set; }
        public string? TransactionStatus { get; set; }
        public decimal? TransactionAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? TransactionDescription { get; set; }
        public Guid? BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public Guid? SellerId { get; set; }
        public string? SellerName { get; set; }

        public Guid? CarbonListingId { get; set; }



    }
}
