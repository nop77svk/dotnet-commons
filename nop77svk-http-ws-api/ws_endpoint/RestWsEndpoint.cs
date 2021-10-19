namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;

    public record RestWsEndpoint : IWebServiceEndpoint
    {
        protected List<string>? Resource { get; set; }
        private ICollection<KeyValuePair<string, string?>>? _query;
        private ICollection<KeyValuePair<string, string?>>? _headers;
        private object? _content;

        public RestWsEndpoint(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
        }

        public HttpMethod HttpMethod { get; init; } = HttpMethod.Get;

        public RestWsEndpoint AddResourceFolder(string? resourceFolder)
        {
            if (string.IsNullOrWhiteSpace(resourceFolder))
                throw new ArgumentOutOfRangeException(nameof(resourceFolder), "Empty resourceFolder to be added");

            if (Resource is null)
                Resource = new List<string>();
            Resource.Add(resourceFolder);

            return this;
        }

        public RestWsEndpoint AddQuery(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Empty key supplied");

            if (_query is null)
                _query = new Dictionary<string, string?>();
            _query?.Add(new KeyValuePair<string, string?>(key, value));

            return this;
        }

        public RestWsEndpoint AddHeader(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Empty key supplied");

            if (_headers is null)
                _headers = new Dictionary<string, string?>();
            _headers.Add(new KeyValuePair<string, string?>(key, value));

            return this;
        }

        public RestWsEndpoint WithContent(object? content)
        {
            _content = content;
            return this;
        }

        public virtual string UriResource
        {
            get => IWebServiceEndpoint.ResourceToString(Resource);
        }

        public bool HasQuery
        {
            get => _query is not null;
        }

        public virtual string UriQuery
        {
            get => IWebServiceEndpoint.QueryToString(_query);
        }

        public bool HasHeaders
        {
            get => _headers is not null;
        }

        public IEnumerable<KeyValuePair<string, string?>> Headers
        {
            get
            {
                if (_headers is not null)
                {
                    foreach (KeyValuePair<string, string?> headerItem in _headers)
                        yield return headerItem;
                }
            }
        }

        public bool HasContent
        {
            get => _content is not null;
        }

        public HttpContent Content
        {
            get
            {
                if (_content is null)
                    throw new ArgumentNullException(nameof(_content), "Serializing NULL content");

                return new StringContent(
                    JsonSerializer.Serialize(_content, new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    }),
                    Encoding.UTF8,
                    "application/json"
                );
            }
        }
    }
}
