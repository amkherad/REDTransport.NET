namespace REDTransport.NET.Tasks
{
    public class TaskTracker
    {
        public ITaskTrackerPersistentStorage PersistentStorage { get; set; }
        
        
        
        
        public TaskTracker(ITaskTrackerPersistentStorage persistentStorage)
        {
            PersistentStorage = persistentStorage;
        }
    }
}