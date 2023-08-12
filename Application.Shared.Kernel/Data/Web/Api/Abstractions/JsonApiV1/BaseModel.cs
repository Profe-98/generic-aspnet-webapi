using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    public abstract class BaseModel
    {
        public virtual new string ToString()
        {
            string response = null;
            Type runtimeType = this.GetType();
            object currentInstace = this;

            var properties = runtimeType.GetProperties();
            if(properties !=null )
            {
                foreach (var property in properties)
                {
                    var value = property.GetValue(currentInstace);
                    string targetValue = property.Name+":";
                    if ((property.PropertyType == typeof(object) || property.PropertyType.BaseType == typeof(BaseModel)) && value != null)
                    {
                        var objectInstance = value;
                        Type objectType = objectInstance.GetType();
                        var methods = objectType.GetMethods();
                        if (methods.Length > 0)
                        {
                            var toStringMethod = methods.ToList().Find(x => x.Name == "ToString");
                            if (toStringMethod != null)
                            {
                                var responseValueFromInvokedMethod = toStringMethod.Invoke(objectInstance, null);
                                if (responseValueFromInvokedMethod != null)
                                    targetValue += (string)responseValueFromInvokedMethod;
                            }
                        }
                    }
                    else if(property.PropertyType == typeof(string)) 
                    {
                        targetValue += String.IsNullOrEmpty((string)value) ? String.Empty : value;
                    }
                    else if (property.PropertyType != typeof(string))
                    {
                        targetValue +=  value != null?value.ToString():String.Empty;
                    }
                    targetValue += ";";
                    response += targetValue;
                }
            }
            return response;
        }
    }
}
