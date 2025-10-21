using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Request.SystemSetting
{
    public class SystemSettingRequest
    {
        public Guid Id { get; set; }
        public string Value { get; set; }

    }
}
