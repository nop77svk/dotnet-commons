namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("privilegeEntry", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_PrivilegeEntry
    {
        [XmlElement("path")]
        public string? Path { get; set; }

        [XmlElement("type")]
        public TdvSoap_ResourceTypeEnum Type { get; set; }

        [XmlArray("privileges")]
        [XmlArrayItem("privilege")]
        public List<TdvSoap_Privilege>? Privileges { get; set; }
    }
}
