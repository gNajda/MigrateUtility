namespace MigrateUtility.Components
{
    public interface IParallelizableDataSource<T> : IDataSource<T>, IParallelizable, IPaginatable<T>
    {
    }
}
