using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigrateUtility.Components
{
    public interface IDataSource<T>
    {
        Task<IEnumerable<T>> GetData();
    }
}