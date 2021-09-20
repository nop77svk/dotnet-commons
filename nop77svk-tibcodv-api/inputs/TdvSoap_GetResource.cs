namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "getResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_GetResource
    {
        [XmlElement("path")]
        public string Path { get; set; } = "/";

        [XmlElement("type")]
        public string? Type { get; set; } = null;

        [XmlElement("detail")]
        public TdvSoap_GetResource_DetailEnum Detail { get; set; } = TdvSoap_GetResource_DetailEnum.Simple;
    }

    public enum TdvSoap_GetResource_DetailEnum
    {
        [XmlEnum(Name = "NONE")]
        None,
        [XmlEnum(Name = "SIMPLE")]
        Simple,
        [XmlEnum(Name = "FULL")]
        Full
    }
}
