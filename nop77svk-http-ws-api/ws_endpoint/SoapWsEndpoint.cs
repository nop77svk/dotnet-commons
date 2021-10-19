namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public record SoapWsEndpoint<TRequest> : IWebServiceEndpoint
    {
        private const string HttpHeaderSoapAction = "SoapAction";

        protected List<string>? Resource { get; set; }
        private readonly string _soapAction;
        private ICollection<KeyValuePair<string, string?>>? _query;
        private ICollection<KeyValuePair<string, string?>>? _headers;
        private readonly TRequest _content;

        public SoapWsEndpoint(string soapAction, TRequest content)
        {
            if (soapAction.Contains('"'))
                throw new ArgumentOutOfRangeException(nameof(soapAction), "Double quote detected in supplied SoapAction");

            _soapAction = soapAction;
            _content = content;
        }

        public HttpMethod HttpMethod { get => HttpMethod.Post; }

        public virtual SoapWsEndpoint<TRequest> AddResourceFolder(string? resourceFolder)
        {
            if (string.IsNullOrWhiteSpace(resourceFolder))
                throw new ArgumentOutOfRangeException(nameof(resourceFolder), "Empty resourceFolder to be added");

            if (Resource is null)
                Resource = new List<string>();
            Resource.Add(resourceFolder);

            return this;
        }

        public SoapWsEndpoint<TRequest> AddQuery(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Empty key supplied");

            if (_query is null)
                _query = new Dictionary<string, string?>();
            _query?.Add(new KeyValuePair<string, string?>(key, value));

            return this;
        }

        public SoapWsEndpoint<TRequest> AddHeader(string? key, string? value)
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

                writer.WriteStartDocument(true);
                writer.WriteStartElement("soap-env", "Envelope", SoapEnvelope_Constants.NamespaceUri);
                writer.WriteStartElement("soap-env", "Body", SoapEnvelope_Constants.NamespaceUri);

                string? reflectedNamespace = ReflectContentTypeForXmlSerializerNamespace();

                XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();
                XmlAttributes attributes = new XmlAttributes()
                {
                    XmlRoot = new XmlRootAttribute()
                    {
                        Namespace = reflectedNamespace,
                        ElementName = _soapAction
                    }
                };
                attributeOverrides.Add(typeof(TRequest), attributes);

                XmlSerializer serializer = new XmlSerializer(typeof(TRequest), attributeOverrides);
                serializer.Serialize(writer, _content);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();

                HttpContent httpContent = new ByteArrayContent(xmlBuffer.ToArray());
                httpContent.Headers.Add(IWebServiceEndpoint.HttpHeaderContentType, "application/xml; charset=US-ASCII");
                return httpContent;
            }
        }

        public static void DeserializeSoapEnvelope(string? soapResponse, out XElement? soapBody)
        {
            if (string.IsNullOrWhiteSpace(soapResponse))
                throw new ArgumentNullException(soapResponse);

            XNamespace soapEnvNs = SoapEnvelope_Constants.NamespaceUri;
            XDocument root = XDocument.Parse(soapResponse);

            XElement? soapEnvelope = root.Element(soapEnvNs + "Envelope");
            if (soapEnvelope is null)
                throw new SoapDeserializationError("SOAP Envelope missing");

            soapBody = soapEnvelope.Element(soapEnvNs + "Body");
            if (soapBody is null)
                throw new SoapDeserializationError("SOAP Body missing");

            XElement? soapFault = soapBody.Element(soapEnvNs + "Fault");
            if (soapFault is not null)
            {
                throw new WebServiceError(string.Join(Environment.NewLine, soapFault
                    .Elements()
                    .Select(node => node.ToString())
                    .Prepend("SOAP call response indicates an error!"))
                );
            }
        }

        public static string? ReflectContentTypeForXmlSerializerNamespace()
        {
            string? reflectedNamespace = typeof(TRequest)
                .GetCustomAttributes(true)
                .Select(x => x switch
                {
                    XmlTypeAttribute typeAttr => typeAttr.Namespace,
                    XmlRootAttribute rootAttr => rootAttr.Namespace,
                    _ => null
                })
                .Where(ns => !string.IsNullOrEmpty(ns))
                .First();

            return reflectedNamespace;
        }

        public async IAsyncEnumerable<TResponse> DeserializeStream<TResponse>(Stream response)
        {
            string reponseContent = await new StreamReader(response).ReadToEndAsync();
            DeserializeSoapEnvelope(reponseContent, out XElement? soapBody);
            if (soapBody is not null)
            {
                foreach (XElement oneBody in soapBody.Elements())
                {
                    TResponse oneResult = DeserializeSoapBodyContent<TResponse>(oneBody);
                    yield return oneResult;
                }
            }
        }

        private TResult DeserializeSoapBodyContent<TResult>(XElement oneBody)
        {
            if (oneBody.Name.Namespace == SoapEnvelope_Constants.NamespaceUri)
                throw new SoapDeserializationError($"SOAP Envelope element \"{oneBody.Name.LocalName}\" found within SOAP Body content");

            string? reflectedNamespace = ReflectContentTypeForXmlSerializerNamespace();
            XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();
            XmlAttributes attributes = new XmlAttributes()
            {
                XmlRoot = new XmlRootAttribute()
                {
                    Namespace = reflectedNamespace,
                    ElementName = typeof(TResult).Name
                }
            };
            attributeOverrides.Add(typeof(TResult), attributes);
            XmlSerializer serializer = new XmlSerializer(typeof(TResult), attributeOverrides);

            StringReader reader = new StringReader(oneBody.ToString());
            TResult? result;
            try
            {
                result = (TResult?)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException e)
            {
                throw new SoapDeserializationError($"Failed to deserialize SOAP Body content with type {typeof(TResult)}:\n{oneBody}", e);
            }

            if (result is null)
                throw new SoapDeserializationError($"SOAP Body content deserialized to NULL:\n{oneBody}");

            return result;
        }
    }
}
