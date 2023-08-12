using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.ComponentModel;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Web.Authentification.JWT
{
    public class JWTModel
    {
        public JWTHeaderModel Header { get; set; }
        public JWTPayloadModel Payload { get; set; }
        public UserModel UserModel
        {
            get
            {
                UserModel userModel = null;
                if (Payload != null)
                {
                    if (Payload != null)
                    {

                        userModel = new UserModel();
                        Type type = userModel.GetType();
                        var allClaimsFromIdenty = Payload.TokenInstance.Claims.ToList();
                        List<PropertyInfo> properties = type.GetProperties().ToList();
                        foreach (PropertyInfo property in properties)
                        {
                            JsonPropertyNameAttribute jsonProperty = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                            if (jsonProperty != null)
                            {
                                Claim foundClaimByJsonName = allClaimsFromIdenty.Find(x => x.Type == jsonProperty.Name);
                                if (foundClaimByJsonName != null)
                                {
                                    Type from = typeof(string);//alle claim values sind string
                                    Type to = property.PropertyType;
                                    TypeConverter typeDescriptor = TypeDescriptor.GetConverter(from);
                                    object fromValue = foundClaimByJsonName.Value;
                                    object value = null;
                                    if (to == typeof(bool) && from == typeof(string))
                                    {
                                        value = bool.Parse(fromValue.ToString());
                                    }
                                    else if (to == typeof(Guid) && from == typeof(string))
                                    {
                                        value = Guid.Parse(fromValue.ToString());
                                    }
                                    else if (to == typeof(DateTime) && from == typeof(string))
                                    {
                                        value = DateTime.Parse(fromValue.ToString());
                                    }
                                    else
                                    {
                                        value = typeDescriptor.ConvertTo(fromValue, to);
                                    }
                                    property.SetValue(userModel, value);
                                }
                            }
                        }
                    }

                }
                return userModel;
            }
        }

    }
}
