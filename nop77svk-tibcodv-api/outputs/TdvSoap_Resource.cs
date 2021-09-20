namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlType("anyResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    [XmlInclude(typeof(TdvSoap_ContainerResource))]
    [XmlInclude(typeof(TdvSoap_ProcedureResource))]
    [XmlInclude(typeof(TdvSoap_TableResource))]
    [XmlInclude(typeof(TdvSoap_LinkResource))]
    [XmlInclude(typeof(TdvSoap_DefinitionSetResource))]
    [XmlInclude(typeof(TdvSoap_PolicyResource))]
    public record TdvSoap_Resource
    {
        [XmlElement("id")]
        public int? Id { get; set; }

        [XmlElement("name")]
        public string? Name { get; set; }

        [XmlElement("path")]
        public string? Path { get; set; }

        [XmlElement("type")]
        public string? Type { get; set; }

        [XmlElement("subtype")]
        public string? SubType { get; set; }

        [XmlElement("enabled")]
        public bool Enabled { get; set; } = true;

        [XmlElement("ownerDomain")]
        public string? OwnerDomain { get; set; }

        [XmlElement("ownerName")]
        public string? OwnerName { get; set; }

        [XmlElement("impactLevel")]
        public string? ImpactLevel { get; set; }
    }
}
