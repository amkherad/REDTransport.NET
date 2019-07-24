using System.Threading.Tasks;

namespace REDTransport.NET.Tasks
{
    public class TaskInfo
    {
        public TaskCompletionSource<Message> TaskCompletionSource { get; }
        
        
        public TaskInfo(TaskCompletionSource<Message> taskCompletionSource)
        {
            TaskCompletionSource = taskCompletionSource;
        }
    }
}