
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL;

namespace WebApiFunction.Application.Model.DataTransferObject
{
    public abstract class DataTransferModelAbstract
    {
        [JsonPropertyName("uuid")]
        public virtual Guid Uuid { get; set; }
        public DataTransferModelAbstract()
        {

        }

        /// <summary>
        /// Mapped the data from the DataTransferModelAbstract Type to a concrete Type of AbstractModel
        /// The Propertynames must be equal to map the data correct from A to B
        /// </summary>
        /// <typeparam name="T">Concrete Implementation of AbstractModel</typeparam>
        /// <returns></returns>
        public T GetMappedConcreteAbstractModel<T>() where T : AbstractModel
        {
            T mappedAbstractModel = Activator.CreateInstance<T>();
            var abstractModelProperties = mappedAbstractModel.GetType().GetProperties()?.ToList();
            var currentInstanceProperties = this.GetType().GetProperties()?.ToList();
            foreach (var prop in currentInstanceProperties)
            {
                PropertyInfo foundInDto = abstractModelProperties.Find(x => x.Name == prop.Name && x.PropertyType == prop.PropertyType);
                if (foundInDto == null)
                    continue;
                var val = prop.GetValue(this);
                foundInDto.SetValue(mappedAbstractModel, val);
            }
            return mappedAbstractModel;
        }
        public string TransformString(string input)
        {
            return input == null ? null : input.ToLower().Trim();
        }
    }
}
