namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType("containerResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    [XmlInclude(typeof(TdvSoap_DataSourceResource))]
    public record TdvSoap_ContainerResource : TdvSoap_Resource
    {
        [XmlElement("childCount")]
        public int ChildCount { get; set; } = 0;
    }
}
