namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot("sqlType", Namespace = TdvSoapNs_Common.NamespacePrefix)]
    public record TdvSoap_CommonSqlType
    {
        [XmlElement("definition")]
        public string? Definition { get; set; }

        [XmlElement("name")]
        public string? Name { get; set; }
    }
}