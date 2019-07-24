using System;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Http;
using REDTransport.NET.Tasks;

namespace REDTransport.NET
{
    public class Transporter
    {
        public HttpClientWrapper HttpClient { get; protected set; }

        public TaskTracker TaskTracker { get; protected set; }

        
        public Transporter(TaskTracker taskTracker)
        {
            if (taskTracker == null) throw new ArgumentNullException(nameof(taskTracker));
            
            HttpClient = new HttpClientWrapper();
            TaskTracker = taskTracker;
        }
        
        public Transporter(ITaskTrackerPersistentStorage persistentStorage)
        {
            if (persistentStorage == null) throw new ArgumentNullException(nameof(persistentStorage));

            HttpClient = new HttpClientWrapper();
            TaskTracker = new TaskTracker(persistentStorage);
        }
        
        // OK-OK
        // Yield
        public async Task<T> SendAsync<T>(EndPointDescriptor endPoint, Message message, CancellationToken cancellationToken)
        {
            //var taskCompletionSource = new TaskCompletionSource<T>();

            //cancellationToken.Register(() => taskCompletionSource.SetCanceled());

//            HttpClient.GetAsync(endPoint, cancellationToken)
//                .ContinueWith(response =>
//                {
//                    
//                    taskCompletionSource.SetResult(default);
//                    
//                }, cancellationToken);
            
            //return taskCompletionSource.Task;


            
        }
    }
}