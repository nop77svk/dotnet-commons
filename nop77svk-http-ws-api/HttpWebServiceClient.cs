namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class HttpWebServiceClient
    {
        private readonly HttpClient _client;

        public HttpWebServiceClient(HttpClient httpClient, string? host, int? port = 80, string? scheme = "http")
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host), "Unspecified HTTP server host");

            _client = httpClient;
            ServerHost = host;
            ServerPort = port;
            ServerScheme = scheme;
        }

        public string ServerHost { get; init; }
        public int? ServerPort { get; init; }
        public string? ServerScheme { get; init; }

        public Action<HttpRequestMessage>? HttpRequestPostprocess { get; set; }
        public Action<HttpResponseMessage>? HttpResponsePostprocess { get; set; }

        public static void AggregateRequestAsString(HttpRequestMessage req, StringBuilder errorText)
        {
            errorText.AppendLine("*** Request method:");
            errorText.AppendLine(req.Method.ToString());

            errorText.AppendLine("*** Request URI:");
            errorText.AppendLine(req.RequestUri?.ToString());

            errorText.AppendLine("*** Request verb:");
            errorText.AppendLine(req.Method.ToString());

            errorText.AppendLine("*** Request headers:");
            foreach (var x in req.Headers)
                errorText.AppendLine($"{x.Key}: {string.Join('|', x.Value)}");

            if (req.Content is not null)
            {
                errorText.AppendLine("*** Request content headers:");
                foreach (var x in req.Content.Headers)
                    errorText.AppendLine($"{x.Key}: {string.Join('|', x.Value)}");

                errorText.AppendLine("*** Request content:");
                errorText.AppendLine(req.Content.ReadAsStringAsync().Result);
            }
        }

        public static void AggregateResponseAsString(HttpResponseMessage response, StringBuilder errorText)
        {
            errorText.AppendLine("*** Reason:");
            errorText.AppendLine(response.ReasonPhrase);

            errorText.AppendLine("*** Response headers:");
            foreach (var x in response.Headers)
                errorText.AppendLine($"{x.Key}: {string.Join('|', x.Value)}");

            if (response.Content is not null)
            {
                errorText.AppendLine("*** Response content headers:");
                foreach (var x in response.Content.Headers)
                    errorText.AppendLine($"{x.Key}: {string.Join('|', x.Value)}");

                errorText.AppendLine("*** Response content:");
                errorText.AppendLine(response.Content.ReadAsStringAsync().Result);
            }
        }

        public static async Task<TResult> DeserializeJsonResponse<TResult>(Stream? json)
        {
            if (json is null)
                throw new ArgumentNullException("No JSON response received");

            var result = await JsonSerializer.DeserializeAsync<TResult>(
                json,
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IncludeFields = true });

            if (result is null)
                throw new ArgumentNullException("Cannot deserialize JSON response");

            return result;
        }

        public static void DeserializeSoapEnvelope(string? soapResponse, out XElement? soapBody, Action<IEnumerable<XElement>>? soapFaultProcessor = null)
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
                if (soapFaultProcessor is not null)
                {
                    soapFaultProcessor(soapFault.Elements());
                }
                else
                {
                    throw new WebServiceError(string.Join(Environment.NewLine, soapFault
                        .Elements()
                        .Select(node => node.ToString())
                        .Prepend("SOAP call response indicates an error!"))
                    );
                }
            }
        }

        public static TBodyContent DeserializeSoapBodyContent<TBodyContent>(XElement soapBodyContent)
        {
            if (soapBodyContent.Name.Namespace == SoapEnvelope_Constants.NamespaceUri)
                throw new SoapDeserializationError($"SOAP Envelope element \"{soapBodyContent.Name.LocalName}\" found within SOAP Body content");

            string? reflectedNamespace = SoapWsEndpoint<TBodyContent>.ReflectContentTypeForXmlSerializerNamespace();
            XmlAttributeOverrides attributeOverrides = new XmlAttributeOverrides();
            XmlAttributes attributes = new XmlAttributes()
            {
                XmlRoot = new XmlRootAttribute()
                {
                    Namespace = reflectedNamespace,
                    ElementName = typeof(TBodyContent).Name
                }
            };
            attributeOverrides.Add(typeof(TBodyContent), attributes);
            XmlSerializer serializer = new XmlSerializer(typeof(TBodyContent), attributeOverrides);

            StringReader reader = new StringReader(soapBodyContent.ToString());
            TBodyContent? result;
            try
            {
                result = (TBodyContent?)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException e)
            {
                throw new SoapDeserializationError($"Failed to deserialize SOAP Body content with type {typeof(TBodyContent)}:\n{soapBodyContent}", e);
            }

            if (result is null)
                throw new SoapDeserializationError($"SOAP Body content deserialized to NULL:\n{soapBodyContent}");

            return result;
        }

        public static AuthenticationHeaderValue GetHeaderForBasicAuthentication(string? userName, string? userPassword)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            if (userPassword is null)
                throw new ArgumentNullException(nameof(userPassword));

            string authenticationString = $"{userName}:{userPassword}";
            string base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            return new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        public async Task<Stream> EndpointGetStream(IWebServiceEndpoint wsep)
        {
            Uri requestUri = CreateUri(wsep);

            using HttpRequestMessage req = new HttpRequestMessage()
            {
                Method = wsep.HttpMethod,
                RequestUri = requestUri
            };

            try
            {
                foreach (KeyValuePair<string, string?> headerItem in wsep.Headers)
                    req.Headers.Add(headerItem.Key, headerItem.Value);

                if (wsep.HasContent)
                    req.Content = wsep.Content;

                HttpRequestPostprocess?.Invoke(req);

                HttpResponseMessage response = await _client.SendAsync(req);
                HttpResponsePostprocess?.Invoke(response);

                if (!response.IsSuccessStatusCode)
                {
                    StringBuilder errorText = new StringBuilder();
                    errorText.AppendLine("HTTP response exception!");
                    AggregateResponseAsString(response, errorText);
                    throw new HttpRequestException(errorText.ToString(), null, response.StatusCode);
                }
                else
                {
                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception e)
            {
                StringBuilder errorText = new StringBuilder();
                errorText.AppendLine("HTTP request exception!");
                AggregateRequestAsString(req, errorText);

                req.Content?.Dispose();

                System.Net.HttpStatusCode? respStatusCode = e is HttpRequestException exception ? exception.StatusCode : null;
                throw new HttpRequestException(errorText.ToString(), e, respStatusCode);
            }
        }

        public async Task<string> EndpointGetString(IWebServiceEndpoint wsep)
        {
            return await new StreamReader(await EndpointGetStream(wsep)).ReadToEndAsync();
        }

        public async Task<TResult> EndpointGetJson<TResult>(IWebServiceEndpoint wsep)
        {
            Stream responseContent = await EndpointGetStream(wsep);
            return await DeserializeJsonResponse<TResult>(responseContent);
        }

        public async IAsyncEnumerable<XElement> EndpointGetSoapBodyXML(IWebServiceEndpoint wsep)
        {
            string? responseContent = await EndpointGetString(wsep);
            DeserializeSoapEnvelope(responseContent, out XElement? soapBody);
            if (soapBody is not null)
            {
                foreach (XElement elm in soapBody.Elements())
                    yield return elm;
            }
        }

        public async IAsyncEnumerable<TResult> EndpointGetSoapBodyObject<TResult>(IWebServiceEndpoint wsep)
        {
            IAsyncEnumerable<XElement> responseContent = EndpointGetSoapBodyXML(wsep);
            await foreach (XElement oneBody in responseContent)
            {
                TResult oneResult = DeserializeSoapBodyContent<TResult>(oneBody);
                yield return oneResult;
            }
        }

        private Uri CreateUri(IWebServiceEndpoint wsep)
        {
            string targetScheme = (string.IsNullOrWhiteSpace(ServerScheme) ? "http" : ServerScheme).ToLower();

            UriBuilder builder = new ()
            {
                Scheme = targetScheme,
                Host = ServerHost,
                Port = ServerPort ?? targetScheme switch
                {
                    null or "" or "http" => 80,
                    "https" => 443,
                    _ => throw new ArgumentException($"Cannot determine target port from request scheme \"{targetScheme}\"")
                },
                UserName = null,
                Password = null,
                Path = wsep.UriResource
            };

            if (wsep is not null)
                builder.Query = wsep.UriQuery;

            return builder.Uri;
        }
    }
}
