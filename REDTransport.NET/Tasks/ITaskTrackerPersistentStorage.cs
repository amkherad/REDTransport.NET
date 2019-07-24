using System.Collections.Generic;
using System.Threading.Tasks;

namespace REDTransport.NET.Tasks
{
    public interface ITaskTrackerPersistentStorage : ICollection<TaskInfo>
    {
        Task<IEnumerable<TaskInfo>> PendingTasks { get; set; }
        
        
    }
}