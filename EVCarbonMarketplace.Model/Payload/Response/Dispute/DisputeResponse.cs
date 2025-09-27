using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Dispute
{
    public class DisputeResponse
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid SendAccountId { get; set; }
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
        public string? EvidenceUrl { get; set; }
        public DateTime CreateAt { get; set; }
    }

}
