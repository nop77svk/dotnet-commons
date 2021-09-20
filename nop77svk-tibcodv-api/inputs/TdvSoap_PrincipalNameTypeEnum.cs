namespace NoP77svk.API.TibcoDV
{
    using System.Xml.Serialization;

    public enum TdvSoap_PrincipalNameTypeEnum
    {
        [XmlEnum("???")]
        Unknown,
        [XmlEnum("USER")]
        User,
        [XmlEnum("GROUP")]
        Group
    }
}