using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher
    {
        public class ResponseFeature : IHttpResponseFeature
        {

            public ResponseFeature()
            {
                StatusCode = 200;
                Headers = new HeaderDictionary();
                Body = Stream.Null;
            }
            

            public int StatusCode { get; set; }
            
            public string ReasonPhrase { get; set; }
            
            public IHeaderDictionary Headers { get; set; }
            
            public Stream Body { get; set; }
            
            
            public bool HasStarted { get; set; }
            
            
            
            public void OnStarting(Func<object, Task> callback, object state)
            {
                
            }

            public void OnCompleted(Func<object, Task> callback, object state)
            {
                
            }
        }
    }
}