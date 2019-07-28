using System;

namespace REDTransport.NET
{
    public struct EndPointDescriptor
    {
        public Uri Uri { get; set; }
        
        public string TargetUniqueId { get; set; }
        
        
        public static implicit operator string(EndPointDescriptor descriptor)
        {
            return descriptor.Uri.ToString();
        }

        public static implicit operator EndPointDescriptor(string uriString)
        {
            return new EndPointDescriptor
            {
                Uri = new Uri(uriString)
            };
        }
        
        public static implicit operator Uri(EndPointDescriptor descriptor)
        {
            return descriptor.Uri;
        }

        public static implicit operator EndPointDescriptor(Uri uri)
        {
            return new EndPointDescriptor
            {
                Uri = uri
            };
        }
    }
}