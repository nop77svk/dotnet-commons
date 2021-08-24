namespace NoP77svk.Web.WS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;

    public interface IWebServiceEndpoint
    {
        public const string HttpHeaderContentType = "Content-Type";

        public HttpMethod HttpMethod { get; }
        public string UriQuery { get; }
        public string UriResource { get; }
        public bool HasHeaders { get; }
        public IEnumerable<KeyValuePair<string, string?>> Headers { get; }
        public bool HasContent { get; }
        public HttpContent Content { get; }

        public static string ResourceToString(IEnumerable<string>? resource)
        {
            if (resource is null)
                return string.Empty;
            else
                return string.Join('/', resource.Select(x => HttpUtility.UrlEncode(x)));
        }

        public static string QueryToString(IEnumerable<KeyValuePair<string, string?>>? query)
        {
            if (query is null)
            {
                return string.Empty;
            }
            else
            {
                return string.Join("&", query
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
        }
    }
}
