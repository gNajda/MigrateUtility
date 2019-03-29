using System.Threading.Tasks;
using MigrateUtility.Components;

namespace MigrateUtility.Process
{
    public class SimpleMigration<T> : IMigrationProcess<T>
    {
        private readonly IDataSource<T> _dataSource;
        private readonly IDataDestination<T> _dataDestination;

        public SimpleMigration(
            IDataSource<T> dataSource, 
            IDataDestination<T> dataDestination)
        {
            _dataSource = dataSource;
            _dataDestination = dataDestination;
        }

        public async Task<OutputWrapper<T>> Execute()
        {
            var dataFromSource = await _dataSource.GetData();
            var saveResult = await _dataDestination.SaveData(dataFromSource);

            return saveResult;
        }
    }
}
