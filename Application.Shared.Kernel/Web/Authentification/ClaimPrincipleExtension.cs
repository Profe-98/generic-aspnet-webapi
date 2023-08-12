using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Web.Authentification
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

            var found = claims.ToList().Find(x=>x.Type == BackendAPIDefinitionsProperties.Claim.ClaimTypeUserUuid);
            if(found==null)
                return Guid.Empty;
            return Guid.Parse(found.Value);
        }
    }
}
