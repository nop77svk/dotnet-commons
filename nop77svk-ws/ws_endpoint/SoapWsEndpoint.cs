namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;

    public record SoapWsEndpoint<TContentType> : IWebServiceEndpoint
    {
        private const string HttpHeaderSoapAction = "SoapAction";

        protected List<string>? Resource { get; set; }
        private readonly string _soapAction;
        private ICollection<KeyValuePair<string, string?>>? _query;
        private ICollection<KeyValuePair<string, string?>>? _headers;
        private readonly TContentType _content;

        public SoapWsEndpoint(string soapAction, TContentType content)
        {
            if (soapAction.Contains('"'))
                throw new ArgumentOutOfRangeException(nameof(soapAction), "Double quote detected in supplied SoapAction");

            _soapAction = soapAction;
            _content = content;
        }

        public HttpMethod HttpMethod { get => HttpMethod.Post; }

        public SoapWsEndpoint<TContentType> AddResourceFolder(string? resourceFolder)
        {
            if (string.IsNullOrWhiteSpace(resourceFolder))
                throw new ArgumentOutOfRangeException(nameof(resourceFolder), "Empty resourceFolder to be added");

            if (Resource is null)
                Resource = new List<string>();
            Resource.Add(resourceFolder);

            return this;
        }

        public SoapWsEndpoint<TContentType> AddQuery(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Empty key supplied");

            if (_query is null)
                _query = new Dictionary<string, string?>();
            _query?.Add(new KeyValuePair<string, string?>(key, value));

            return this;
        }

        public SoapWsEndpoint<TContentType> AddHeader(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Empty key supplied");

            if (_headers is null)
                _headers = new Dictionary<string, string?>();
            _headers?.Add(new KeyValuePair<string, string?>(key, value));

            return this;
        }

        public virtual string UriResource
        {
            get => Resource is null ? string.Empty : string.Join('/', Resource.Select(x => HttpUtility.UrlEncode(x)));
        }

        public bool HasQuery
        {
            get => _query is not null;
        }

        public virtual string UriQuery
        {
            get => _query is null ? string.Empty : string.Join("&", _query
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .Select(x => HttpUtility.UrlEncode(x.Key)
                    + x.Value switch
                    {
                        null => string.Empty,
                        "" => "=",
                        _ => "=" + HttpUtility.UrlEncode(x.Value)
                    }
                )
            );
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

                yield return new (HttpHeaderSoapAction, '"' + _soapAction.Replace("\"", null) + '"');
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

                SoapEnvelopePOCO<TContentType> soapedContent = new SoapEnvelopePOCO<TContentType>()
                {
                    Body = new SoapEnvelopeBodyPOCO<TContentType>()
                    {
                        Content = _content
                    }
                };

                using MemoryStream xmlBuffer = new ();
                using XmlWriter writer = XmlWriter.Create(xmlBuffer, new XmlWriterSettings()
                {
                    CheckCharacters = true,
                    CloseOutput = true,
                    Encoding = Encoding.ASCII,
                    Indent = true,
                    IndentChars = "   ",
                    OmitXmlDeclaration = false,
                    WriteEndDocumentOnClose = true
                });

                XmlSerializer serializer = new XmlSerializer(typeof(SoapEnvelopePOCO<TContentType>));
                serializer.Serialize(writer, soapedContent);
                writer.Flush();
                writer.Close();

                HttpContent httpContent = new ByteArrayContent(xmlBuffer.ToArray());
                httpContent.Headers.Add(IWebServiceEndpoint.HttpHeaderContentType, "application/xml; charset=US-ASCII");
                return httpContent;
            }
        }
    }
}
