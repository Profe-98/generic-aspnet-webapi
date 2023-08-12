using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Shared.Kernel.Application.Model.DataTransferObject;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL
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
