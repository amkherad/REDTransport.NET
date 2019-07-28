using System.Collections.Generic;

namespace REDTransport.NET.Tasks
{
    public class TaskTracker<T>
    {
        public ITaskTrackerPersistentStorage<T> PersistentStorage { get; protected set; }

        
        private readonly Dictionary<string, TaskInfo<T>> _taskMappings;


        public TaskTracker(
            ITaskTrackerPersistentStorage<T> persistentStorage
        )
        {
            _taskMappings = new Dictionary<string, TaskInfo<T>>();
            PersistentStorage = persistentStorage;
        }

        public bool TryGetTaskByUniqueId(string correlationId, out TaskInfo<T> taskInfo)
        {
            return _taskMappings.TryGetValue(correlationId, out taskInfo);
        }


        public void Track(string correlationId, TaskInfo<T> trackedTask)
        {
            _taskMappings.Add(correlationId, trackedTask);
        }
    }
}