using System.Threading.Tasks;

namespace MigrateUtility.Process
{
    public interface IMigrationProcess<T>
    {
        Task<OutputWrapper<T>> Execute();
    }
}