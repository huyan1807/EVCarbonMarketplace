using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.Certificate
{
    public class CertificateResponse
    {
        public Guid Id { get; set; }
        public Guid? CarbonCreditId { get; set; }
        public int? SerialNumber { get; set; }
        public string? CertificateUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? BuyerId { get; set; }
    }
}
