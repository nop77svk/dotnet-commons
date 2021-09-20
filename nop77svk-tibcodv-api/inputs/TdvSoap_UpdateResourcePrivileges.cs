namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "updateResourcePrivileges", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_UpdateResourcePrivileges
    {
        [XmlElement("updateRecursively")]
        public bool UpdateRecursively { get; set; } = false;

        [XmlElement("updateDependenciesRecursively")]
        public bool UpdateDependenciesRecursively { get; set; } = false;

        [XmlElement("updateDependentsRecursively")]
        public bool UpdateDependentsRecursively { get; set; } = false;

        [XmlArray("privilegeEntries")]
        [XmlArrayItem("privilegeEntry")]
        public TdvSoap_PrivilegeEntry[]? PrivilegeEntries { get; set; }

        [XmlElement("mode")]
        public TdvSoap_UpdateResourcePrivileges_ModeEnum Mode { get; set; } = TdvSoap_UpdateResourcePrivileges_ModeEnum.OverwriteAppend;
    }

    public enum TdvSoap_UpdateResourcePrivileges_ModeEnum
    {
        [XmlEnum(Name = "OVERWRITE_APPEND")]
        OverwriteAppend,
        [XmlEnum(Name = "SET_EXACTLY")]
        SetExactly
    }
}
