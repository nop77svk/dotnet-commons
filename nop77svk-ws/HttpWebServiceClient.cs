namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class HttpWebServiceClient
    {
        private readonly HttpClient _client;

        public HttpWebServiceClient(HttpClient httpClient, string host, int? port = 80, string? scheme = "http")
        {
            _client = httpClient;
            ServerHost = host;
            ServerPort = port;
            ServerScheme = scheme;
        }

        public string ServerHost { get; init; }

        public int? ServerPort { get; init; }

        public string? ServerScheme { get; init; }

        public static async Task<T> DeserializeJsonResponse<T>(Stream? json)
        {
            if (json is null)
                throw new ArgumentNullException("No JSON response received");

            var result = await JsonSerializer.DeserializeAsync<T>(
                json,
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IncludeFields = true });

            if (result is null)
                throw new ArgumentNullException("Cannot deserialize JSON response");

            return result;
        }

        public static async Task<ICollection<T>> DeserializeJsonArrayResponse<T>(Stream? json)
        {
            return await DeserializeJsonResponse<List<T>>(json);
        }

        public async Task<Stream> CallEndpointStreamed(IWebServiceEndpoint wsep)
        {
            Uri requestUri = CreateUri(wsep);

            try
            {
                using HttpRequestMessage req = new ()
                {
                    Method = wsep.HttpMethod,
                    RequestUri = requestUri
                };

                foreach (KeyValuePair<string, string?> headerItem in wsep.Headers)
                    req.Headers.Add(headerItem.Key, headerItem.Value);

                if (wsep.HasContent)
                    req.Content = wsep.Content;

                try
                {
                    HttpResponseMessage response = await _client.SendAsync(req);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("*** Request URI:\n" + req.RequestUri);
                        Console.WriteLine("*** Request verb: " + req.Method.ToString());

                        Console.WriteLine("*** Request headers:");
                        foreach (var x in req.Headers)
                            Console.WriteLine($"{x.Key}: \"{string.Join('|', x.Value)}\"");

                        if (req.Content is not null)
                        {
                            Console.WriteLine("*** Request content headers:");
                            foreach (var x in req.Content.Headers)
                                Console.WriteLine($"{x.Key}: \"{string.Join('|', x.Value)}\"");

                            Console.WriteLine("*** Request content:");
                            Console.WriteLine(await req.Content.ReadAsStringAsync());
                        }

                        Console.WriteLine("*** Response headers:");
                        foreach (var x in response.Headers)
                            Console.WriteLine($"{x.Key}: \"{string.Join('|', x.Value)}\"");

                        if (response.Content is not null)
                        {
                            Console.WriteLine("*** Response content headers:");
                            foreach (var x in response.Content.Headers)
                                Console.WriteLine($"{x.Key}: \"{string.Join('|', x.Value)}\"");

                            Console.WriteLine("*** Response content:");
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                        }

                        throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
                    }
                    else
                    {
                        return await response.Content.ReadAsStreamAsync();
                    }
                }
                finally
                {
                    req.Content?.Dispose();
                }
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException($"Failed to \"{wsep.HttpMethod}\" the \"{requestUri}\" endpoint", e);
            }
        }

        public async Task<string> CallEndpointString(IWebServiceEndpoint wsep)
        {
            return await new StreamReader(await CallEndpointStreamed(wsep)).ReadToEndAsync();
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
