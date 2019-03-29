using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MigrateUtility.Components;

namespace MigrateUtility.Process
{
    internal class ParallelMigration<T> : IMigrationProcess<T>
    {
        private readonly IParallelizableDataSource<T> _dataSource;
        private readonly IDataDestination<T> _dataDestination;
        private readonly int _readTaskCount;
        private readonly int _batchSize;
        private readonly int _writeTaskCount;
        private readonly ConcurrentQueue<IEnumerable<T>> _dataQueue = new ConcurrentQueue<IEnumerable<T>>();
        private bool _isReadingFinished;
        private readonly bool _failOnSingleWriteError;

        internal ParallelMigration(
            IParallelizableDataSource<T> dataSource, 
            IDataDestination<T> dataDestination, 
            int readTaskCount, 
            int batchSize, 
            int writeTaskCount, 
            bool failOnSingleWriteError)
        {
            _dataSource = dataSource;
            _dataDestination = dataDestination;
            _readTaskCount = readTaskCount;
            _batchSize = batchSize;
            _writeTaskCount = writeTaskCount;
            _failOnSingleWriteError = failOnSingleWriteError;
        }

        public async Task<OutputWrapper<T>> Execute()
        {
            var dataSize = _dataSource.GetDataSize();
            var pageSizePerTask = dataSize / _readTaskCount;
            var rest = dataSize - (pageSizePerTask * _readTaskCount);

            var pageSizeForEachTask = Enumerable.Repeat(_readTaskCount - 1, pageSizePerTask).ToList();
            pageSizeForEachTask.Add(pageSizePerTask + rest);

            var readTasks = pageSizeForEachTask.Select((x, i) => CreateReadTask(i * x, x)).ToArray();
            var writeTasks = Enumerable.Repeat(CreateWriteTask(), _writeTaskCount).ToArray();
            Task.WaitAll(readTasks);
            _isReadingFinished = true;
            var outputList = await Task.WhenAll(writeTasks);
            var output = outputList.Aggregate((x, y) => x + y);
            return output;
        }

        private async Task CreateReadTask(int taskIndex, int pageSize)
        {
            var retrievedDataCount = 0;
            var nextBatchIndex = taskIndex * pageSize;
            var nextBatchSize = pageSize > _batchSize ? _batchSize : pageSize;
            int leftDataCount;
            do
            {
                var data = (await _dataSource.GetPage(nextBatchIndex, nextBatchSize)).ToList();
                _dataQueue.Enqueue(data);
                retrievedDataCount+= data.Count;
                nextBatchIndex = nextBatchIndex + nextBatchSize;
                leftDataCount = pageSize - retrievedDataCount;
                nextBatchSize = leftDataCount > _batchSize ? _batchSize : leftDataCount;
            } while (leftDataCount != 0);
        }

        private async Task<OutputWrapper<T>> CreateWriteTask()
        {
            var output = new OutputWrapper<T>();

            while (!_isReadingFinished || _isReadingFinished && !_dataQueue.IsEmpty)
            {
                if (!_dataQueue.TryDequeue(out var data)) continue;
                try
                {
                    output += await _dataDestination.SaveData(data);
                }
                catch (Exception)
                {
                    if (_failOnSingleWriteError)
                    {
                        throw;
                    }
                }
            }

            return output;
        }
    }
}
