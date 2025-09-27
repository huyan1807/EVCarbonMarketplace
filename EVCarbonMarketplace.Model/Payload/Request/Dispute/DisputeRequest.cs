using EVCarbonMarketplace.Model.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.Dispute
{
    public class DisputeRequest
    {
        public Guid TransactionId { get; set; }
        public DisputeTypeEnum? Type { get; set; }
        public string? Description { get; set; }
        public IFormFile EvidenceUrl { get; set; }
    }
}
