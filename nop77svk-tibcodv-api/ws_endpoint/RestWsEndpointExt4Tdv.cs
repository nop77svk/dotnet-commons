namespace NoP77svk.API.TibcoDV
{
    using NoP77svk.Web.WS;

    public static class RestWsEndpointExt4Tdv
    {
        public static RestWsEndpoint AddTdvQuery(this RestWsEndpoint self, string? key, bool? value)
        {
            return self.AddQuery(key, value?.ToString().ToLower());
        }
    }
}
