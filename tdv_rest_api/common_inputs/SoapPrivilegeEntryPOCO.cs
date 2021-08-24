namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType(AnonymousType = false, Namespace = SoapUpdateResourcePrivileges_Constants.XmlNamespaceURI, TypeName = "privilegeEntry")]
    public class SoapPrivilegeEntryPOCO
    {
        [XmlAttribute("path")]
        public string? Path { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("privileges")]
        public SoapPrivilegePOCO[]? Privileges { get; set; }
    }
}
