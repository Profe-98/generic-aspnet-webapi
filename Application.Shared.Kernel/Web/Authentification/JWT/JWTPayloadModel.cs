using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Shared.Kernel.Web.Authentification.JWT
{
    public class JWTPayloadModel
    {

        public JwtSecurityToken TokenInstance { get; set; }

    }
}
