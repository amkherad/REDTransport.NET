using System.Collections.Generic;

namespace REDTransport.NET.Tasks
{
    public class TaskTracker
    {
        public ITaskTrackerPersistentStorage PersistentStorage { get; set; }

        private Dictionary<int, TaskInfo> _taskMappings;  
        
        
        
        public TaskTracker(ITaskTrackerPersistentStorage persistentStorage)
        {
            _taskMappings = new Dictionary<int, TaskInfo>();
            PersistentStorage = persistentStorage;
        }

        public bool TryGetTaskByMessage(Message message, out TaskInfo taskInfo)
        {
            var messageId = Message.GetMessageId(message).GetHashCode();

            return _taskMappings.TryGetValue(messageId, out taskInfo);
        }
    }
}