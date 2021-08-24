namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Net.Http;
    using NoP77svk.Web.WS;

    public record TdvRestWsEndpoint : RestWsEndpoint
    {
        public int ApiVersion { get; init; }

        public string ApiFeature { get; init; }

        public TdvRestWsEndpoint(HttpMethod httpMethod, string apiFeature, int apiVersion)
            : base(httpMethod)
        {
            ApiVersion = apiVersion;
            ApiFeature = apiFeature;
        }

        private IEnumerable<string> OverrideResource()
        {
            yield return "rest";
            yield return ApiFeature;
            yield return $"v{ApiVersion}";

            if (Resource is not null)
            {
                foreach (string? resFolder in Resource)
                    yield return resFolder;
            }
        }

        public override string UriResource
        {
            get => IWebServiceEndpoint.ResourceToString(OverrideResource());
        }
    }
}
