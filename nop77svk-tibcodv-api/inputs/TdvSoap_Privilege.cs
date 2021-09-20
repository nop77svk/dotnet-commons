namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot("privilege", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public class TdvSoap_Privilege
    {
        [XmlElement("domain")]
        public string? Domain { get; init; }

        [XmlElement("name")]
        public string? Name { get; init; }

        [XmlElement("nameType")]
        public TdvSoap_PrincipalNameTypeEnum NameType { get; init; }

        [XmlElement("privs")]
        public string? Privs { get; init; }
        // public string? combinedPrivs { get; set; }
        // public string? inheritedPrivs { get; set; }
    }
}
