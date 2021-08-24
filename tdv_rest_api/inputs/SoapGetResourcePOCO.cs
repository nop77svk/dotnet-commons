namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "getResource", Namespace = SoapGetResource_Constants.XmlNamespaceURI)]
    public record SoapGetResourcePOCO
    {
        [XmlElement("path")]
        public string Path { get; set; } = "/";

        [XmlElement("type")]
        public string? Type { get; set; } = null;

        [XmlElement("detail")]
        public SoapGetResource_Detail Detail { get; set; } = SoapGetResource_Detail.Simple;
    }

    public enum SoapGetResource_Detail
    {
        [XmlEnum(Name = "NONE")]
        None,
        [XmlEnum(Name = "SIMPLE")]
        Simple,
        [XmlEnum(Name = "FULL")]
        Full
    }

    public struct SoapGetResource_Constants
    {
        public const string XmlNamespaceURI = "http://www.compositesw.com/services/system/admin/resource";
        public const string XmlNamespacePrefix = "resource";
    }
}
