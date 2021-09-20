namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("linkResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_LinkResource
        : TdvSoap_Resource
    {
        [XmlElement("targetType")]
        public string? TargetType { get; set; }
    }
}
