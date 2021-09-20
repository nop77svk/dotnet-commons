namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("getResourceResponse", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_GetResourceResponse
    {
        [XmlArray("resources")]
        [XmlArrayItem("resource")]
        public List<TdvSoap_Resource>? Resources { get; set; }
    }
}
