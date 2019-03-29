namespace MigrateUtility.Configuration
{
    public class ProcessConfiguration
    {
        public ProcessType ProcessType = ProcessType.Simple;
        public ParallelConfiguration ParallelConfiguration = new ParallelConfiguration();
        public BatchConfiguration BatchConfiguration = new BatchConfiguration();
    }
}
