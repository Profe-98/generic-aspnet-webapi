using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Infrastructure.Database.Dapper.Converter
{
    internal class DapperStringToGuidConverter : SqlMapper.TypeHandler<Guid>
    {
        public DapperStringToGuidConverter()
        {

        }

        public override Guid Parse(object value)
        {
            Guid responseValue = Guid.Empty;
            if (value != null)
            {
                try
                {
                    responseValue = Guid.Parse(value?.ToString());
                }
                catch (Exception ex)
                {

                }
            }
            return responseValue;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {

            parameter.Value = value.ToString();
        }
    }
    public static class DapperTypeConverterHandler
    {
        public static void UseStringToGuidConversion()
        {

            SqlMapper.AddTypeHandler(new DapperStringToGuidConverter());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
        }

    }
}
