namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("procedureResource", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_ProcedureResource : TdvSoap_Resource
    {
        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        public List<TdvSoap_ProcedureParameter>? Parameters { get; set; }
    }
}
