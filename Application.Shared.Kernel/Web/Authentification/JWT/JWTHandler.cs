﻿using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;
using Application.Shared.Kernel.Data.Format.Json;

namespace Application.Shared.Kernel.Web.Authentification.JWT
{
    public class JWTHandler : IJWTHandler
    {
        #region Private
        private readonly IScopedJsonHandler _jsonHandler = null;
        #endregion
        #region Public

        #endregion
        #region Ctor
        public JWTHandler(IScopedJsonHandler jsonHandler)
        {
            _jsonHandler = jsonHandler;
            HashAlgorithms = new Dictionary<GeneralDefs.JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { GeneralDefs.JwtHashAlgorithm.RS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { GeneralDefs.JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { GeneralDefs.JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        }
        #endregion

        #region Methods

        public string GenerateJwtToken(UserModel userModel, string secret, DateTime expires, bool withoutRoleClaims = false)
        {
            byte[] key = Encoding.UTF8.GetBytes(secret);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserUuid, userModel.Uuid.ToString()),
                    new Claim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUser, userModel.User.ToString()),
                    new Claim("expires_time",expires.ToString()),
                });

            if (!withoutRoleClaims)
            {

                foreach (RoleModel role in userModel.AvaibleRoles)
                {
                    Claim claimRole = new Claim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, role.Name);
                    claimsIdentity.AddClaim(claimRole);
                }
            }
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                TokenType = "JWT",
                Issuer = "http://localhost:5010/",
                Audience = "http://localhost:5010/",
                NotBefore = DateTime.Now,
                IssuedAt = DateTime.Now,
                Subject = claimsIdentity,

                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenStr = tokenHandler.WriteToken(token);

            return tokenStr;
        }
        private Dictionary<GeneralDefs.JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;

        public string Encode(object payload, string key, GeneralDefs.JwtHashAlgorithm algorithm)
        {
            return Encode(payload, Encoding.UTF8.GetBytes(key), algorithm);
        }

        public string Encode(object payload, byte[] keyBytes, GeneralDefs.JwtHashAlgorithm algorithm)
        {
            var segments = new List<string>();
            var header = new { alg = algorithm.ToString(), typ = "JWT" };

            byte[] headerBytes = Encoding.UTF8.GetBytes(_jsonHandler.JsonSerialize(header));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(_jsonHandler.JsonSerialize(payload));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(@"{"iss":"761326798069-r5mljlln1rd4lrbhg75efgigp36m78j5@developer.gserviceaccount.com","scope":"https://www.googleapis.com/auth/prediction","aud":"https://accounts.google.com/o/oauth2/token","exp":1328554385,"iat":1328550785}");

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            byte[] signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
            segments.Add(Base64UrlEncode(signature));

            return string.Join(".", segments.ToArray());
        }


        public JWTModel Decode(string token, string key)
        {
            return Decode(token, key, true);
        }

        public JWTModel Decode(string token, string key, bool verify)
        {

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                return null;

            string header = parts[0];
            string payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);

            string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            JWTHeaderModel headerData = _jsonHandler.JsonDeserialize<JWTHeaderModel>(headerJson);
            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new JsonConverter.JsonBoolConverter());
            JWTPayloadModel payloadData = _jsonHandler.JsonDeserialize<JWTPayloadModel>(payloadJson, options);
            JwtSecurityToken tData = tokenHandler.ReadJwtToken(token);

            payloadData.TokenInstance = tData;
            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var algorithm = headerData.Algorithm;

                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                if (decodedCrypto != decodedSignature)
                {
                    throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
                }
            }

            return new JWTModel { Header = headerData, Payload = payloadData };
        }

        private static GeneralDefs.JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "RS256": return GeneralDefs.JwtHashAlgorithm.RS256;
                case "HS384": return GeneralDefs.JwtHashAlgorithm.HS384;
                case "HS512": return GeneralDefs.JwtHashAlgorithm.HS512;
                default: throw new InvalidOperationException("Algorithm not supported.");
            }
        }

        // from JWT spec
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        // from JWT spec
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
        #endregion JsonWebToken
    }
}
