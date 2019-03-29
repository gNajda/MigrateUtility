using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigrateUtility.Components
{
    public interface IPaginatable<T>
    {
        Task<IEnumerable<T>> GetPage(int startIndex, int length);
    }
}
