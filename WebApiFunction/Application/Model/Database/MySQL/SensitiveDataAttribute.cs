using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;
using WebApiFunction.Configuration;

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
                            bool foundClaim = claims.Find(x => x.Type == BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole && x.Value == claimNeeded) != null;
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
            System.Diagnostics.Debug.WriteLine("prop:" + prop.Name + " (" + prop.PropertyType.Name + "; GetGenericTypeDefinition=" + (prop.PropertyType.IsGenericType?prop.PropertyType.GetGenericTypeDefinition():"") + "), from value: " + value.GetType().Name + "");
            var interfacesFromType = prop.PropertyType.GetInterfaces(); 
            if (prop.PropertyType == typeof(object) || prop.Name.Contains(".") || (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                System.Diagnostics.Debug.WriteLine("--> prop:" + prop.Name + " ("+prop.PropertyType.Name+"), from value: " + value.GetType().Name + "");
                var subValue = prop.GetValue(value, null);

                if (subValue != null)
                {
                    if (subValue is IList)
                    {
                        IEnumerable enumerable = subValue as IEnumerable;
                        var data = enumerable.OfType<object>().ToList();
                        for (int i=0;i<data.Count();i++)
                        {

                            var resp = SetSensitivePropertiesToDefault(data[i], claims);

                            data[i] = resp;
                        }
                        prop.SetValue(value, subValue);
                    }
                    else
                    {

                        var resp = SetSensitivePropertiesToDefault(subValue, claims);
                        prop.SetValue(value, resp);
                    }
                }
            }
        }
    }
}
