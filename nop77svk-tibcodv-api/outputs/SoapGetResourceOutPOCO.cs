namespace NoP77svk.API.TibcoDV
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    /*
<resource:getResourceResponse xmlns:resource="http://www.compositesw.com/services/system/admin/resource" xmlns:common="http://www.compositesw.com/services/system/util/common" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <resource:resources>
    <resource:resource xsi:type="resource:dataSourceResource">
      <resource:name>Erste_DV_TU</resource:name>
      <resource:path>/services/databases/Erste_DV_TU</resource:path>
      <resource:type>DATA_SOURCE</resource:type>
      <resource:subtype>RELATIONAL_DATA_SOURCE</resource:subtype>
      <resource:id>187154</resource:id>
      <resource:changeId>15091</resource:changeId>
      <resource:ownerDomain>composite</resource:ownerDomain>
      <resource:ownerName>h50suob_temp</resource:ownerName>
      <resource:impactLevel>NONE</resource:impactLevel>
      <resource:enabled>true</resource:enabled>
      <resource:childCount>8</resource:childCount>
      <resource:dataSourceType>COMPOSITE_DATABASE</resource:dataSourceType>
    </resource:resource>
  </resource:resources>
</resource:getResourceResponse>
    */
    [XmlRoot(ElementName = "getResourceResponse", Namespace = SoapGetResource_Constants.XmlNamespaceURI)]
    public record SoapGetResourceResponsePOCO
    {
        [XmlAttribute("resources")]
        public IEnumerable<SoapGetResourceResponse_Resource>? Resources { get; set; }
    }

    [XmlRoot(ElementName = "resource", Namespace = SoapGetResource_Constants.XmlNamespaceURI)]
    public record SoapGetResourceResponse_Resource
    {
        [XmlAttribute("id")]
        public int? Id { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("path")]
        public string? Path { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("subtype")]
        public string? SubType { get; set; }

        [XmlAttribute("enabled")]
        public bool? Enabled { get; set; }

        [XmlAttribute("dataSourceType")]
        public string? DataSourceType { get; set; }

        [XmlAttribute("ownerDomain")]
        public string? OwnerDomain { get; set; }

        [XmlAttribute("ownerName")]
        public string? OwnerName { get; set; }
    }
}
