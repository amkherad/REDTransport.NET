using System;
using System.Threading;
using System.Threading.Tasks;

namespace REDTransport.NET
{
    public static class TransporterExtensions
    {
        public static Task<T> GetAsync<T>(
            this Transporter transporter,
            EndPointDescriptor endPoint,
            CancellationToken cancellationToken)
        {
            if (transporter == null) throw new ArgumentNullException(nameof(transporter));
            
            return transporter.SendAsync<T>(endPoint, null, cancellationToken);
        }
    }
}