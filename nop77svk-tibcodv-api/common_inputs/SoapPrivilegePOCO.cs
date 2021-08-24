namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType(AnonymousType = false, Namespace = SoapUpdateResourcePrivileges_Constants.XmlNamespaceURI, TypeName = "privilege")]
    public class SoapPrivilegePOCO
    {
        [XmlAttribute("domain")]
        public string? Domain { get; init; }

        [XmlAttribute("name")]
        public string? Name { get; init; }

        [XmlAttribute("nameType")]
        public string? NameType { get; init; }

        [XmlAttribute("privs")]
        public string? Privs { get; init; }

        // public string? combinedPrivs { get; set; }
        // public string? inheritedPrivs { get; set; }
    }
}
