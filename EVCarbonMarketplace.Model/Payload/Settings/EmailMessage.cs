using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Settings
{
    public class EmailMessage
    {
        public string ToAddress { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}
