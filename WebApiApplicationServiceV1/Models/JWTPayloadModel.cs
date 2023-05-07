using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace WebApiApplicationService.Models
{
    public class JWTPayloadModel
    {

        public JwtSecurityToken TokenInstance { get; set; }

    }
}
