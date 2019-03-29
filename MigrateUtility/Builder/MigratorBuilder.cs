using System;
using MigrateUtility.Components;
using MigrateUtility.Configuration;
using MigrateUtility.Process;

namespace MigrateUtility.Builder
{
    public class MigratorBuilder<T>
    {
        private ProcessConfiguration _processConfiguration = new ProcessConfiguration();
        private IDataSource<T> _dataSource;
        private IDataDestination<T> _dataDestination;

        public MigratorBuilder<T> WithDataSource(IDataSource<T> dataSource)
        {
            _dataSource = dataSource;
            return this;
        }

        public MigratorBuilder<T> WithDataDestination(IDataDestination<T> dataSource)
        {
            _dataDestination = dataSource;
            return this;
        }

        public MigratorBuilder<T> SetupProcessConfiguration(Func<ProcessConfiguration, ProcessConfiguration> configuration)
        {
            _processConfiguration = configuration(_processConfiguration);
            return this;
        }

        public IMigrationProcess<T> Build()
        {
            IMigrationProcess<T> newMigrationProcess = null;

            switch (_processConfiguration.ProcessType)
            {
                case ProcessType.Parallel:
                    newMigrationProcess = new ParallelMigration<T>(
                        (IParallelizableDataSource<T>)_dataSource,
                        _dataDestination, 
                        _processConfiguration.ParallelConfiguration.ReadTaskCount,
                        _processConfiguration.ParallelConfiguration.BatchSize,
                        _processConfiguration.ParallelConfiguration.WriteTaskCount,
                        _processConfiguration.ParallelConfiguration.FailOnSingleWriteError);
                    break;
                case ProcessType.Batch:
                    newMigrationProcess = new BatchMigration<T>(
                            (IPaginatable<T>)_dataSource,
                            _dataDestination, 
                            _processConfiguration.BatchConfiguration.BatchSize,
                            _processConfiguration.BatchConfiguration.FailOnSingleWriteError
                        );
                    break;
                case ProcessType.Simple:
                    newMigrationProcess = new SimpleMigration<T>(_dataSource, _dataDestination);
                    break;
            };

            return newMigrationProcess;
        }
    }
}