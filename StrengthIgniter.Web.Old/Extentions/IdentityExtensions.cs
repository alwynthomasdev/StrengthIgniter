using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Extentions
{
    public static class IdentityExtensions
    {
        public static Guid GetNameIdentifier(this ClaimsPrincipal @this)
        {
            return @this.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => Guid.Parse(c.Value))
                .SingleOrDefault();
        }
    }
}
