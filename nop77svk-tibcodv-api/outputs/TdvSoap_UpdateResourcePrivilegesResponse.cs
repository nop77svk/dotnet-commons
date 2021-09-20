namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot("updateResourcePrivilegesResponse", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_UpdateResourcePrivilegesResponse
    {
        public string? Body { get; set; }
    }
}
