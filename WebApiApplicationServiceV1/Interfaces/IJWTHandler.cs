using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService
{
    public interface IJWTHandler
    {
        public string GenerateJwtToken(UserModel userModel, string secret, DateTime expires,bool forRegistration = false);
        public string Encode(object payload, string key, GeneralDefs.JwtHashAlgorithm algorithm);
        public string Encode(object payload, byte[] keyBytes, GeneralDefs.JwtHashAlgorithm algorithm);
        public JWTModel Decode(string token, string key);
        public JWTModel Decode(string token, string key, bool verify);
    }
}
