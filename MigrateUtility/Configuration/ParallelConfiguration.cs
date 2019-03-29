namespace MigrateUtility.Configuration
{
    public class ParallelConfiguration
    {
        public int BatchSize;
        public int ReadTaskCount;
        public int WriteTaskCount;
        public bool FailOnSingleWriteError;
    }
}
