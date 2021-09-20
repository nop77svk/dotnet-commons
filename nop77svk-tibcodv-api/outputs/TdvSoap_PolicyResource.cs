namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType("policyResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_PolicyResource
        : TdvSoap_Resource
    {
    }
}
