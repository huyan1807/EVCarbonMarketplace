using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Withdraw
{
    public class WithdrawResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountHolder { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Description { get; set; }  
        public string? ProofUrl { get; set; }    
    }
}
