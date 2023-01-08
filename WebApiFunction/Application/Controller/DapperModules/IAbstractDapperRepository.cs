using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySql;

namespace WebApiFunction.Application.Controller.DapperModules
{
    public interface IAbstractDapperRepository<T> where T : AbstractModel
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}
