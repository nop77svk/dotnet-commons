namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType("definitionSetResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_DefinitionSetResource
        : TdvSoap_Resource
    {
    }
}
