using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Payload.Response.User
{
    public class GetUserResponse
    {
        public Guid? AccountId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }

        public string? UserName { get; set; }


        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }



    }
}
