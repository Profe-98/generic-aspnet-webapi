using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Controller.Modules;

namespace WebApiFunction.Application.Model.Database.MySQL
{
    public static class AbstractModelExtension
    {

        public static IAbstractBackendModule<T> GetModule<T>(this IServiceProvider serviceProvider)
            where T : AbstractModel
        {
            return serviceProvider.GetService<IAbstractBackendModule<T>>();
        }
    }
}
