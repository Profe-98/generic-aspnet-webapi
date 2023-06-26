using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Web.Authentification
{
    public static class ClaimPrincipleExtension
    {
        public static Guid GetUuidFromClaims(this ClaimsPrincipal principal)
        {
            if(principal == null)
                return Guid.Empty;

            var claims = principal.Claims;
            if(claims == null||claims.Count() ==0)
                return Guid.Empty;

            var found = claims.ToList().Find(x=>x.Type == "uuid");
            if(found==null)
                return Guid.Empty;
            return Guid.Parse(found.Value);
        }
    }
}
