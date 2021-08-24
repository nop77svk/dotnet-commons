namespace NoP77svk.API.TibcoDV
{
    using System.Linq;
    using NoP77svk.Web.WS;

    public record TdvSoapWsEndpoint<TContentType> : SoapWsEndpoint<TContentType>
    {
        public TdvSoapWsEndpoint(string soapAction, TContentType content)
            : base(soapAction, content)
        {
        }

        public override string UriResource
        {
            get => Resource is null ? string.Empty : IWebServiceEndpoint.ResourceToString(Resource
                .Prepend("services")
                .Append(Resource.Last() + "Port.ws")
            );
        }
    }
}
