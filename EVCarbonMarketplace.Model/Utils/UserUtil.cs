using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace EVCarbonMarketplace.Model.Utils
{
    public static class UserUtil
    {
        public static Guid? GetAccountId(HttpContext httpContext)
        {
            if (httpContext == null || httpContext.User == null)
            {
                return null;
            }
            foreach (var claim in httpContext.User.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }
            //var nameIdentifierClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.NameId);
            var nameIdentifierClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (nameIdentifierClaim == null)
            {
                return null;
            }
           

            if (!Guid.TryParse(nameIdentifierClaim.Value, out Guid accountId))
            {
                throw new BadHttpRequestException(nameIdentifierClaim.Value);

            }
            return accountId;
        }
    }
}
