using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Transaction
{
    public class TransactionUserResponse
    {
        public Guid Id { get; set; }
        public Guid? CarbonListingId { get; set; }  
        public Guid? CarbonCreditId { get; set; }  

        public string? Type { get; set; }           
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Description { get; set; }
    }
}
