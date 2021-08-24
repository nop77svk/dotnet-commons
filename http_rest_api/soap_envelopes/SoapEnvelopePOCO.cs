namespace NoP77svk.Web.WS
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "Envelope", Namespace = SoapEnvelope_Constants.NamespaceUri)]
    public record SoapEnvelopePOCO<THeader, TContent>
    {
        [XmlElement(ElementName = "Header", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public THeader? Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public SoapEnvelopeBodyPOCO<TContent>? Body { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = SoapEnvelope_Constants.NamespaceUri)]
    public record SoapEnvelopePOCO<TContent>
    {
        [XmlElement(ElementName = "Body", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public SoapEnvelopeBodyPOCO<TContent>? Body { get; set; }
    }

    public record SoapEnvelopeWithFaultPOCO<THeader, TContent, TFault>
    {
        [XmlElement(ElementName = "Header", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public THeader? Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public SoapEnvelopeBodyPOCO<TContent, TFault>? Body { get; set; }
    }

    public record SoapEnvelopeWithFaultPOCO<TContent, TFault>
    {
        [XmlElement(ElementName = "Body", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public SoapEnvelopeBodyPOCO<TContent, TFault>? Body { get; set; }
    }

    public record SoapEnvelopeBodyPOCO<TContent, TFault>
    {
        public TContent? Content { get; set; }

        [XmlElement(ElementName = "Fault", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public TFault? Fault { get; set; }
    }

    public record SoapEnvelopeBodyPOCO<TContent>
    {
        public TContent? Content { get; set; }

        [XmlElement(ElementName = "Fault", Namespace = SoapEnvelope_Constants.NamespaceUri)]
        public string? Fault { get; set; }
    }

    public struct SoapEnvelope_Constants
    {
        public const string NamespaceUri = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string NamespacePrefix = "soap";
    }
}
