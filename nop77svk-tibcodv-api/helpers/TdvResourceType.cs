namespace NoP77svk.API.TibcoDV
{
    using System;

    public class TdvResourceType
    {
        public string? WsType { get; init; }
        public string? WsSubType { get; init; }

        public TdvResourceType(string? wsType, string? wsSubtype)
        {
            WsType = wsType;
            WsSubType = wsSubtype;
        }

        public TdvResourceTypeEnum Type { get => CalcResourceType(WsType, WsSubType, null); }

        /// <see cref="https://docs.tibco.com/pub/tdv/8.4.0/doc/html/StudioHelp/index.html#page/StudioHelp/Ch_3_OperationsList.TDV%2520Resource%2520Types%2520and%2520Subtypes.html"/>
        public static TdvResourceTypeEnum CalcResourceType(string? wsType, string? wsSubType, string? wsTargetType)
        {
            // 2do! cache the mapping via Dictionary<Tuple<string?, string?, string?>, TdvResourceTypeEnum>
            return (wsType, wsSubType, wsTargetType) switch
            {
                (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.FolderContainer, _) => TdvResourceTypeEnum.Folder,
                (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.CatalogContainer, _) => TdvResourceTypeEnum.PublishedCatalog,
                (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.SchemaContainer, _) => TdvResourceTypeEnum.PublishedSchema,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.RelationalDataSource, _) => TdvResourceTypeEnum.DataSourceRelational,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.CompositeWebService, _) => TdvResourceTypeEnum.DataSourceCompositeWebService,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.FileDataSource, _) => TdvResourceTypeEnum.DataSourceFile,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.PoiExcelDataSource, _) => TdvResourceTypeEnum.DataSourceExcel,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.XmlFileDataSource, _) => TdvResourceTypeEnum.DataSourceXmlFile,
                (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.WsdlDataSource, _) => TdvResourceTypeEnum.DataSourceWsWsdl,
                (TdvResourceTypeConst.Table, TdvResourceSubtypeConst.DatabaseTable, _) => TdvResourceTypeEnum.Table,
                (TdvResourceTypeConst.Table, TdvResourceSubtypeConst.SqlTable, _) => TdvResourceTypeEnum.View,
                (TdvResourceTypeConst.Procedure, TdvResourceSubtypeConst.SqlScriptProcedure, _) => TdvResourceTypeEnum.StoredProcedureSQL,
                (TdvResourceTypeConst.Procedure, _, _) => TdvResourceTypeEnum.StoredProcedureOther,
                (TdvResourceTypeConst.Link, TdvResourceSubtypeConst.None, TdvResourceTypeConst.Table) => TdvResourceTypeEnum.PublishedTableOrView,
                (TdvResourceTypeConst.DefinitionSet, _, _) => TdvResourceTypeEnum.DefinitionSet,
                (TdvResourceTypeConst.Model, TdvResourceSubtypeConst.None, _) => TdvResourceTypeEnum.Model,
                _ => throw new ArgumentOutOfRangeException(nameof(wsType) + ":" + nameof(wsSubType) + ":" + nameof(wsTargetType), $"Unrecognized combination of resource type \"{wsType}\", subtype \"{wsSubType}\" and target type \"{wsTargetType}\"")
            };
        }

        public static ValueTuple<string, string> CalcWsResourceTypes(TdvResourceTypeEnum type)
        {
            return type switch
            {
                TdvResourceTypeEnum.Folder => new (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.FolderContainer),
                TdvResourceTypeEnum.PublishedCatalog => new (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.CatalogContainer),
                TdvResourceTypeEnum.PublishedSchema => new (TdvResourceTypeConst.Container, TdvResourceSubtypeConst.SchemaContainer),
                TdvResourceTypeEnum.DataSourceRelational => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.RelationalDataSource),
                TdvResourceTypeEnum.DataSourceCompositeWebService => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.CompositeWebService),
                TdvResourceTypeEnum.DataSourceFile => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.FileDataSource),
                TdvResourceTypeEnum.DataSourceExcel => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.PoiExcelDataSource),
                TdvResourceTypeEnum.DataSourceXmlFile => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.XmlFileDataSource),
                TdvResourceTypeEnum.DataSourceWsWsdl => new (TdvResourceTypeConst.DataSource, TdvResourceSubtypeConst.WsdlDataSource),
                TdvResourceTypeEnum.Table => new (TdvResourceTypeConst.Table, TdvResourceSubtypeConst.DatabaseTable),
                TdvResourceTypeEnum.View => new (TdvResourceTypeConst.Table, TdvResourceSubtypeConst.SqlTable),
                TdvResourceTypeEnum.StoredProcedureSQL => new (TdvResourceTypeConst.Procedure, TdvResourceSubtypeConst.SqlScriptProcedure),
                TdvResourceTypeEnum.PublishedTableOrView => new (TdvResourceTypeConst.Link, TdvResourceSubtypeConst.None),
                TdvResourceTypeEnum.Model => new (TdvResourceTypeConst.Model, TdvResourceSubtypeConst.None),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unrecognized resource type \"{type}\"")
            };
        }
    }
}
