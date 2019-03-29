using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MigrateUtility.Components;

namespace MigrateUtility.Process
{
    internal class BatchMigration<T> : IMigrationProcess<T>
    {
        private readonly IPaginatable<T> _dataSource;
        private readonly IDataDestination<T> _dataDestination;
        private readonly int _batchSize;
        private readonly bool _failOnSingleWriteError;

        internal BatchMigration(IPaginatable<T> dataSource,
            IDataDestination<T> dataDestination,
            int batchSize,
            bool failOnSingleWriteError)
        {
            _dataSource = dataSource;
            _dataDestination = dataDestination;
            _batchSize = batchSize;
            _failOnSingleWriteError = failOnSingleWriteError;
        }

        public async Task<OutputWrapper<T>> Execute()
        {
            var index = 0;
            var output = new OutputWrapper<T>();
            List<T> dataFromDataSource;

            do
            {
                dataFromDataSource = (await _dataSource.GetPage(index, _batchSize)).ToList();
                var saveDataResult = await _dataDestination.SaveData(dataFromDataSource);

                if (!saveDataResult.ISuccess && _failOnSingleWriteError)
                {
                    throw new Exception();
                }

                output += saveDataResult;
                index += _batchSize;

            } while (dataFromDataSource.Count == _batchSize);

            return output;
        }
    }
}
