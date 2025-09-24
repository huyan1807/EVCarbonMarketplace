using EVCarbonMarketplace.Model.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.Withdraw
{
    public class UpdateWithdrawRequest
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public IFormFile? ProofUrl { get; set; }
        public WithdrawEnum? Status { get; set; }
    }
}
