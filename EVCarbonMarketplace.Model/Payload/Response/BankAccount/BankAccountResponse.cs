using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.BankAccount
{
    public class BankAccountResponse
    {
        public Guid Id { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankAccountHolder { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreateAt { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
    }
}
