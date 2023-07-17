using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Application.Model.Database.MySQL
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal class SensitiveDataAttribute : Attribute
    {
        public readonly string ClaimsValuesForDataAccess;
        /// <summary>
        /// Kommaseparierte Claims Values für den Claim-Key: user_role
        /// </summary>
        /// <param name="claimsValuesForDataAccess"></param>
        public SensitiveDataAttribute(string claimsValuesForDataAccess)
        {
            ClaimsValuesForDataAccess = claimsValuesForDataAccess;
        }
    }
    public static class SensitiveDataAttributeExtension
    {
        public static object SetSensitivePropertiesToDefault(this object value, List<Claim> claims)
        {
            var props = value.GetType().GetProperties();
            foreach (var prop in props.ToList().FindAll(x=>x.CanWrite))
            {

                var sensitiveAttr = prop.GetCustomAttribute<SensitiveDataAttribute>();
                var val = prop.GetValue(value, null);   
                if(val != null)
                {

                    if (sensitiveAttr != null)
                    {

                        string[] claimsThatNeededForAccess = sensitiveAttr.ClaimsValuesForDataAccess.Split(",");
                        bool hasAccessToProp = false;
                        foreach (string claimNeeded in claimsThatNeededForAccess)
                        {
                            bool foundClaim = claims.Find(x => x.Type == "user_role" && x.Value == claimNeeded) != null;
                            if (foundClaim)
                            {
                                hasAccessToProp = true;
                                break;
                            }
                        }
                        if (!hasAccessToProp)
                        {
                            prop.SetValue(value, null);
                        }
                        else
                        {

                            ProcessNestedType(prop, value, claims);
                        }

                    }
                    else
                    {
                        ProcessNestedType(prop, value, claims);
                    }
                }


            }
            return value;
        }
        public static void ProcessNestedType(PropertyInfo prop, object value,List<Claim> claims)
        {

            if (prop.PropertyType == typeof(object) || prop.Name.Contains("."))
            {
                var subValue = prop.GetValue(value, null);
                if (subValue != null)
                {
                    var resp = SetSensitivePropertiesToDefault(subValue, claims);
                    prop.SetValue(value, resp);
                }
            }
        }
    }
}
