using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigrateUtility.Components
{
    public interface IDataDestination<T> 
    {
        Task<OutputWrapper<T>> SaveData(IEnumerable<T> data);
    }
}
