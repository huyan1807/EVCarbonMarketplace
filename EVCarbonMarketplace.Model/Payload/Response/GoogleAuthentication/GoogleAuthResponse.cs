using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication
{
    public class GoogleAuthResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }

        public string Avatar { get; set; }
    }
}
