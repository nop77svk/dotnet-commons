namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("tableResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_TableResource
        : TdvSoap_Resource
    {
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public List<TdvSoap_TableColumn>? Columns { get; set; }
    }
}
