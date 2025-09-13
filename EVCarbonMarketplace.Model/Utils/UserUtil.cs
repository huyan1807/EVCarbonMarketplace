using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
