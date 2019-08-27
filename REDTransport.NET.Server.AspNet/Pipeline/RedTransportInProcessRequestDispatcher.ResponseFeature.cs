using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Http;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher
    {
        public class ResponseFeature : IHttpResponseFeature
        {
            public ResponseMessage ResponseMessage { get; }

            private IHeaderDictionary _headers;
            
            public ResponseFeature(ResponseMessage responseMessage)
            {
                ResponseMessage = responseMessage;
                _headers = new HeaderCollectionWrapper(responseMessage.Headers);
            }


            public int StatusCode
            {
                get => ResponseMessage.StatusCode;
                set => ResponseMessage.StatusCode = value;
            }

            public string ReasonPhrase
            {
                get => ResponseMessage.StatusMessage;
                set => ResponseMessage.StatusMessage = value;
            }

            public IHeaderDictionary Headers
            {
                get => _headers;
                set => _headers = value;
            }

            public Stream Body
            {
                get => ResponseMessage.Body;
                set => ResponseMessage.Body = value;
            }


            public bool HasStarted
            {
                get => true;
                set => value = value;
            }
            
            
            
            public void OnStarting(Func<object, Task> callback, object state)
            {
                
            }

            public void OnCompleted(Func<object, Task> callback, object state)
            {
                
            }
        }
    }
}