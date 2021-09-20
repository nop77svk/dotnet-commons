namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot("column", Namespace = TdvSoapNs_Resource.XmlNamespacePrefix)]
    public record TdvSoap_TableColumn
    {
        [XmlElement("name")]
        public string? Name { get; set; }

        [XmlElement("dataType")]
        public TdvSoap_CommonSqlType? DataType { get; set; }
    }
}