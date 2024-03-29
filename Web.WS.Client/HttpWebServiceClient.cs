﻿namespace NoP77svk.Web.WS
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

            errorText.AppendLine("*** (end of request headers)");
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

            errorText.AppendLine("*** (end of response headers)");
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

        public async Task EndpointCall(IWebServiceEndpoint wsep)
        {
            using HttpResponseMessage response = await EndpointGetHttpResponse(wsep);
        }

        public async IAsyncEnumerable<TResult> EndpointGetObject<TResult>(IWebServiceEndpoint wsep)
        {
            using HttpResponseMessage response = await EndpointGetHttpResponse(wsep);
            using Stream responseContent = await response.Content.ReadAsStreamAsync();
            await foreach (TResult result in wsep.DeserializeStream<TResult>(responseContent))
                yield return result;
        }

        public async Task<string> EndpointGetString(IWebServiceEndpoint wsep)
        {
            using HttpResponseMessage response = await EndpointGetHttpResponse(wsep);
            string result = await response.Content.ReadAsStringAsync();
            return result;
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

        private async Task<HttpResponseMessage> EndpointGetHttpResponse(IWebServiceEndpoint wsep)
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

                using Task<HttpResponseMessage> sendAsyncTask = _client.SendAsync(req);
                HttpResponseMessage response = await sendAsyncTask; // 2do! memory/resource leak! HttpResponseMessage : IDisposable
                HttpResponsePostprocess?.Invoke(response);

                if (!response.IsSuccessStatusCode)
                {
                    StringBuilder errorText = new StringBuilder();
                    errorText.AppendLine("HTTP response exception!");
                    AggregateResponseAsString(response, errorText);
                    throw new HttpRequestException(errorText.ToString(), null, response.StatusCode);
                }

                return response;
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
    }
}
