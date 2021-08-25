namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "updateResourcePrivileges", Namespace = SoapUpdateResourcePrivileges_Constants.XmlNamespaceURI)]
    public record SoapUpdateResourcePrivilegesPOCO
    {
        [XmlAttribute("updateRecursively")]
        public bool UpdateRecursively { get; set; } = false;

        [XmlAttribute("updateDependenciesRecursively")]
        public bool UpdateDependenciesRecursively { get; set; } = false;

        [XmlAttribute("updateDependentsRecursively")]
        public bool UpdateDependentsRecursively { get; set; } = false;

        [XmlAttribute("privilegeEntries")]
        public SoapPrivilegeEntryPOCO[]? PrivilegeEntries { get; set; }

        [XmlAttribute("mode")]
        public SoapUpdateResourcePrivileges_Mode Mode { get; set; } = SoapUpdateResourcePrivileges_Mode.OverwriteAppend;
    }

    public enum SoapUpdateResourcePrivileges_Mode
    {
        [XmlEnum(Name = "OVERWRITE_APPEND")]
        OverwriteAppend,
        [XmlEnum(Name = "SET_EXACTLY")]
        SetExactly
    }

    public struct SoapUpdateResourcePrivileges_Constants
    {
        public const string XmlNamespaceURI = "http://www.compositesw.com/services/system/admin/resource";
        public const string XmlNamespacePrefix = "resource";
    }
}
