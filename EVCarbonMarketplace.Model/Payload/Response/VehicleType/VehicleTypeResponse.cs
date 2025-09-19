using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.VehicleType
{
    public class VehicleTypeResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateAt { get; set; }

    }
}
