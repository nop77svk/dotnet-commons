namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    [XmlRoot("parameter", Namespace = TdvSoapNs_Resource.XmlNamespaceURI)]
    public record TdvSoap_ProcedureParameter
    {
        [XmlElement("name")]
        public string? Name { get; set; }

        [XmlElement("dataType")]
        public TdvSoap_CommonSqlType? DataType { get; set; }

        [XmlElement("direction")]
        public TdvSoap_ParameterDirectionEnum Direction { get; set; } = TdvSoap_ParameterDirectionEnum.In;

        [XmlElement("isNullable")]
        public TdvSoap_ParameterNullabilityEnum IsNullableX { get; set; } = TdvSoap_ParameterNullabilityEnum.IsNullable;

        [XmlIgnore]
        public bool IsNullable
        {
            get => IsNullableX == TdvSoap_ParameterNullabilityEnum.IsNullable;
            set
            {
                IsNullableX = value ? TdvSoap_ParameterNullabilityEnum.IsNotNullable : TdvSoap_ParameterNullabilityEnum.IsNullable;
            }
        }
    }

    public enum TdvSoap_ParameterDirectionEnum
    {
        [XmlEnum("IN")]
        In,
        [XmlEnum("OUT")]
        Out
    }

    public enum TdvSoap_ParameterNullabilityEnum
    {
        [XmlEnum("IS_NULLABLE")]
        IsNullable,
        [XmlIgnore] // 2do!
        IsNotNullable
    }
}
