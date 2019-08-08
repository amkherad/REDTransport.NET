using System.Net.Http;
using REDTransport.NET.Message;

namespace REDTransport.NET.Http
{
    public interface IHttpConverter
    {
        HttpRequestMessage ToHttpRequestMessage(RequestMessage requestMessage);

        HttpResponseMessage ToHttpResponseMessage(ResponseMessage responseMessage);


        RequestMessage FromHttpRequestMessage(HttpRequestMessage requestMessage);

        ResponseMessage FromHttpResponseMessage(HttpResponseMessage responseMessage);
    }
}