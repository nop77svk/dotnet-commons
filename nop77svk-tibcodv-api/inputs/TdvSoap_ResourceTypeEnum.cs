namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    public enum TdvSoap_ResourceTypeEnum
    {
        [XmlEnum("???")]
        Unknown,
        [XmlEnum(TdvResourceTypeConst.Container)]
        Container,
        [XmlEnum(TdvResourceTypeConst.DataSource)]
        DataSource,
        [XmlEnum(TdvResourceTypeConst.Table)]
        Table,
        [XmlEnum(TdvResourceTypeConst.Procedure)]
        Procedure,
        [XmlEnum(TdvResourceTypeConst.Link)]
        Link,
        [XmlEnum(TdvResourceTypeConst.DefinitionSet)]
        DefinitionSet,
        [XmlEnum(TdvResourceTypeConst.Model)]
        Model
    }
}
