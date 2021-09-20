namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType("dataSourceResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_DataSourceResource : TdvSoap_ContainerResource
    {
        [XmlElement("dataSourceType")]
        public string? DataSourceType { get; set; }
    }
}
