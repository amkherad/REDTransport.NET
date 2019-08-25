using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using REDTransport.NET.Http;

namespace REDTransport.NET.Messages
{
    [DebuggerDisplay("{RequestMethod} \"{Uri}\" {ProtocolVersion}, Headers={Headers.Count}")]
    public class RequestMessage
    {
        private string _scheme;
        private string _host;
        private string _pathBase;
        private string _path;
        private string _queryString;
        private string _rawTarget;
#nullable enable
        private Uri? _uri;
#nullable disable


        public RequestMessage()
        {
        }

        public RequestMessage(
            string protocolVersion,
            Uri uri,
            string requestMethod,
            HeaderCollection headers,
            Stream body
        )
        {
            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));

            Uri = uri ?? throw new ArgumentNullException(nameof(uri));

            RequestMethod = requestMethod ?? throw new ArgumentNullException(nameof(requestMethod));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Body = body;
        }

        public RequestMessage(
            string protocolVersion,
            string scheme,
            string host,
            string pathBase,
            string path,
            string queryString,
            string rawTarget,
            string requestMethod,
            HeaderCollection headers,
            Stream body
        )
        {
            Version = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));

            _scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _pathBase = pathBase ?? throw new ArgumentNullException(nameof(pathBase));
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _queryString = queryString ?? throw new ArgumentNullException(nameof(queryString));
            _rawTarget = rawTarget ?? throw new ArgumentNullException(nameof(rawTarget));

            RequestMethod = requestMethod ?? throw new ArgumentNullException(nameof(requestMethod));
            Headers = headers ?? new HeaderCollection(HttpHeaderType.RequestHeader);
            Body = body;
        }


        public string Scheme
        {
            get => _scheme;
            set
            {
                _scheme = value;
                _uri = null;
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                _uri = null;
            }
        }

        public string PathBase
        {
            get => _pathBase;
            set
            {
                _pathBase = value;
                _uri = null;
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                _uri = null;
            }
        }

        public string QueryString
        {
            get => _queryString;
            set
            {
                _queryString = value;
                _uri = null;
            }
        }

        public string RawTarget
        {
            get => _rawTarget;
            set
            {
                _rawTarget = value;
                //_uri = null;
            }
        }

        public string Version { get; set; }

        public string RequestMethod { get; set; }

        public HeaderCollection Headers { get; set; }

        public Stream Body { get; set; }


        public string ProtocolVersion
        {
            get => $"HTTP/{Version}";
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                var parts = value.Split('/');

                if (parts.Length != 2)
                {
                    throw new InvalidOperationException();
                }

                //_scheme = parts[0];
                Version = parts[1];
            }
        }

        public Uri Uri
        {
            get
            {
                if (_uri != null)
                {
                    return _uri;
                }

                var path = string.IsNullOrWhiteSpace(_pathBase) ? _path : $"{_pathBase}/{_path}";
                var query = _queryString;
                if (!string.IsNullOrWhiteSpace(query) && !query.StartsWith('?'))
                {
                    query = '?' + query;
                }

                string hostPath;
                if (_host.EndsWith('/') || path.StartsWith('/'))
                {
                    hostPath = _host + path;
                }
                else
                {
                    hostPath = $"{_host}/{path}";
                }

                var uri = new Uri($"{_scheme}://{hostPath}{query}");

                _uri = uri;

                return _uri;
            }
            set
            {
                _uri = value;

                _scheme = value.Scheme;
                _host = value.Host;
                _path = value.AbsolutePath;
                _pathBase = null;
                _queryString = value.Query;
            }
        }


        public string CorrelationId
        {
            get => Headers.Single(ProtocolConstants.REDCorrelationIdHeaderName);
            set => Headers.Set(ProtocolConstants.REDCorrelationIdHeaderName, value);
        }
    }
}