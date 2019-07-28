using System.Collections.Generic;
using System.Threading.Tasks;

namespace REDTransport.NET.Tasks
{
    public interface ITaskTrackerPersistentStorage<T> : ICollection<TaskInfo<T>>
    {
        Task<IEnumerable<TaskInfo<T>>> PendingTasks { get; set; }
        
        
    }
}