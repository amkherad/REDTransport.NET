using System;
using System.Threading.Tasks;

namespace REDTransport.NET.Tasks
{
    public class TaskInfo<T>
    {
        public TaskCompletionSource<T> TaskCompletionSource { get; }
        
        public TimeSpan? Timeout { get; }
        
        
        public TaskInfo(TaskCompletionSource<T> taskCompletionSource)
        {
            TaskCompletionSource = taskCompletionSource;
        }
        public TaskInfo(TaskCompletionSource<T> taskCompletionSource, TimeSpan? timeout)
        {
            TaskCompletionSource = taskCompletionSource;
            Timeout = timeout;
        }
    }
}