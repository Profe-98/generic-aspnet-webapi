using System;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Web.Authentification.JWT
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
