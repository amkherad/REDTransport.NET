using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Http
{
    /// <summary>
    /// Provides a customizable mechanism to convert RED messages to platform specific messages.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRedTransportMessageConverter<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Determines whether a request is a RED request. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsRedRequest(TRequest request);

        /// <summary>
        /// Determines whether a response is a RED response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsRedResponse(TResponse request);


        /// <summary>
        /// Converts a <see cref="RequestMessage"/> to <see cref="TRequest"/>.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TRequest> ToRequestAsync(RequestMessage requestMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Converts a <see cref="ResponseMessage"/> to <see cref="TResponse"/>.
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> ToResponseAsync(ResponseMessage responseMessage, CancellationToken cancellationToken);


        /// <summary>
        /// Converts a <see cref="TRequest"/> to <see cref="RequestMessage"/>.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RequestMessage> FromRequestAsync(TRequest requestMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Converts a <see cref="TResponse"/> to <see cref="ResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseMessage> FromResponseAsync(TResponse responseMessage, CancellationToken cancellationToken);


        /// <summary>
        /// Copies a <see cref="RequestMessage"/> to a <see cref="TRequest"/>.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="target"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CopyRequestMessageToTarget(TRequest target, RequestMessage requestMessage,
            CancellationToken cancellationToken);

        /// <summary>
        /// Copies a <see cref="ResponseMessage"/> to a <see cref="TResponse"/>.
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="target"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CopyResponseMessageToTarget(TResponse target, ResponseMessage responseMessage,
            CancellationToken cancellationToken);


        /// <summary>
        /// Copies a <see cref="TRequest"/> to a <see cref="RequestMessage"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="requestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CopyTargetToRequestMessage(RequestMessage requestMessage, TRequest target,
            CancellationToken cancellationToken);

        /// <summary>
        /// Copies a <see cref="TResponse"/> to a <see cref="ResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="target"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CopyTargetToResponseMessage(ResponseMessage responseMessage, TResponse target,
            CancellationToken cancellationToken);
    }
}