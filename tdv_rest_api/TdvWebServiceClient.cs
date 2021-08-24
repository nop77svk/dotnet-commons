namespace NoP77svk.API.TibcoDV
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NoP77svk.IO;
    using NoP77svk.Web.WS;

    /// <summary>
    /// Implementation of a few Tibco DV 8.4 server's REST/SOAP API calls as specified
    /// in <see cref="https://docs.tibco.com/pub/tdv/8.4.0/doc/html/StudioHelp/index.html#page/StudioHelp/Ch_7_REST-API.TDV%2520Server%2520REST%2520APIs.html#"/>Tibco DV 8.4 REST API Guide</see>
    /// and <see cref="https://docs.tibco.com/pub/tdv/8.4.0/doc/html/StudioHelp/index.html#page/StudioHelp/Ch_3_OperationsList.Operations%2520Reference.html#"/>Tibco DV 8.4 WS API Guide</see>.
    /// </summary>
    public class TdvWebServiceClient
    {
        public const char FolderDelimiter = '/';

        private readonly HttpWebServiceClient _httpRestClient;

        public TdvWebServiceClient(HttpWebServiceClient httpRestClient, int apiVersion = 1)
        {
            _httpRestClient = httpRestClient;
            ApiVersion = apiVersion;
        }

        public int ApiVersion { get; init; }

        /// <see cref="https://docs.tibco.com/pub/tdv/8.4.0/doc/html/StudioHelp/index.html#page/StudioHelp/Ch_3_OperationsList.TDV%2520Resource%2520Types%2520and%2520Subtypes.html"/>
        public static TdvResourceTypeEnum DetermineResourceType(string? resourceType, string? resourceSubType, string? targetResourceType)
        {
            // 2do! cache the mapping via Dictionary<Tuple<string?, string?, string?>, TdvResourceTypeEnum>
            return (resourceType, resourceSubType, targetResourceType) switch
            {
                (TdvResourceType.Container, TdvResourceSubtype.FolderContainer, _) => TdvResourceTypeEnum.Folder,
                (TdvResourceType.Container, TdvResourceSubtype.CatalogContainer, _) => TdvResourceTypeEnum.PublishedCatalog,
                (TdvResourceType.Container, TdvResourceSubtype.SchemaContainer, _) => TdvResourceTypeEnum.PublishedSchema,
                (TdvResourceType.DataSource, TdvResourceSubtype.RelationalDataSource, _) => TdvResourceTypeEnum.DataSourceRelational,
                (TdvResourceType.DataSource, TdvResourceSubtype.CompositeWebService, _) => TdvResourceTypeEnum.DataSourceCompositeWebService,
                (TdvResourceType.DataSource, TdvResourceSubtype.FileDataSource, _) => TdvResourceTypeEnum.DataSourceFile,
                (TdvResourceType.DataSource, TdvResourceSubtype.PoiExcelDataSource, _) => TdvResourceTypeEnum.DataSourceExcel,
                (TdvResourceType.DataSource, TdvResourceSubtype.XmlFileDataSource, _) => TdvResourceTypeEnum.DataSourceXmlFile,
                (TdvResourceType.DataSource, TdvResourceSubtype.WsdlDataSource, _) => TdvResourceTypeEnum.DataSourceWsWsdl,
                (TdvResourceType.Table, TdvResourceSubtype.DatabaseTable, _) => TdvResourceTypeEnum.Table,
                (TdvResourceType.Table, TdvResourceSubtype.SqlTable, _) => TdvResourceTypeEnum.View,
                (TdvResourceType.Procedure, TdvResourceSubtype.SqlScriptProcedure, _) => TdvResourceTypeEnum.StoredProcedureSQL,
                (TdvResourceType.Procedure, _, _) => TdvResourceTypeEnum.StoredProcedureOther,
                (TdvResourceType.Link, TdvResourceSubtype.None, TdvResourceType.Table) => TdvResourceTypeEnum.PublishedTableOrView,
                (TdvResourceType.DefinitionSet, _, _) => TdvResourceTypeEnum.DefinitionSet,
                (TdvResourceType.Model, TdvResourceSubtype.None, _) => TdvResourceTypeEnum.Model,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType) + ":" + nameof(resourceSubType) + ":" + nameof(targetResourceType), $"Unrecognized combination of resource type \"{resourceType}\", subtype \"{resourceSubType}\" and target type \"{targetResourceType}\"")
            };
        }

        public static TdvResourceTypeEnum DetermineResourceType(ContainerContentsOutPOCO resource)
        {
            try
            {
                return DetermineResourceType(resource.Type, resource.SubType, resource.TargetType);
            }
            catch (Exception e)
            {
                throw new ArgumentOutOfRangeException($"Error while determining type of resource \"{resource.Path}\"", e);
            }
        }

        public async Task<string> GetResourceInfo(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string response = await _httpRestClient.CallEndpointString(new TdvSoapWsEndpoint<SoapGetResourcePOCO>(
                "getResource",
                new SoapGetResourcePOCO()
                {
                    Path = path,
                    Type = null,
                    Detail = SoapGetResource_Detail.Simple
                })
                    .AddResourceFolder("system")
                    .AddResourceFolder("admin")
                    .AddResourceFolder("resource")
            );

            return response;
        }

        public async Task<IEnumerable<ContainerContentsOutPOCO>> RetrieveContainerContents(string? path, TdvResourceTypeEnum resourceType)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string jsonResourceType = resourceType switch
            {
                TdvResourceTypeEnum.Folder or TdvResourceTypeEnum.PublishedCatalog or TdvResourceTypeEnum.PublishedSchema => TdvResourceType.Container.ToLower(),
                TdvResourceTypeEnum.DataSourceRelational or TdvResourceTypeEnum.DataSourceCompositeWebService => TdvResourceType.DataSource.ToLower(),
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType.ToString())
            };

            Stream response = await _httpRestClient.CallEndpointStreamed(new TdvRestWsEndpoint(HttpMethod.Get, TdvRestEndpointFeature.Resource, 1)
                .AddResourceFolder("children")
                .AddQuery("path", path)
                .AddQuery("type", jsonResourceType)
            );
            return await HttpWebServiceClient.DeserializeJsonArrayResponse<ContainerContentsOutPOCO>(response);
        }

        public async IAsyncEnumerable<ContainerContentsOutPOCO> RetrieveContainerContentsRecursive(string? path, TdvResourceTypeEnum resourceType)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            List<Task<IEnumerable<ContainerContentsOutPOCO>>> subfolderReaders = new ();
            subfolderReaders.Add(RetrieveContainerContents(path, resourceType));

            while (subfolderReaders.Any())
            {
                Task<IEnumerable<ContainerContentsOutPOCO>> finishedSubfolderReader = await Task.WhenAny(subfolderReaders);
                subfolderReaders.Remove(finishedSubfolderReader);

                foreach (ContainerContentsOutPOCO folderItem in finishedSubfolderReader.Result
                    .Where(folderItem => folderItem.TdvResourceType is TdvResourceTypeEnum.Folder
                        or TdvResourceTypeEnum.PublishedCatalog
                        or TdvResourceTypeEnum.PublishedSchema
                        or TdvResourceTypeEnum.DataSourceCompositeWebService
                        or TdvResourceTypeEnum.DataSourceRelational)
                )
                    subfolderReaders.Add(RetrieveContainerContents(folderItem.Path, folderItem.TdvResourceType));

                foreach (ContainerContentsOutPOCO folderItem in finishedSubfolderReader.Result)
                    yield return folderItem;
            }
        }

        public async Task<ICollection<TableColumnsOutPOCO>> RetrieveTableColumns(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            Stream response = await _httpRestClient.CallEndpointStreamed(new TdvRestWsEndpoint(HttpMethod.Get, TdvRestEndpointFeature.Resource, 1)
                .AddResourceFolder("table")
                .AddResourceFolder("columns")
                .AddQuery("path", path)
                .AddQuery("type", "table")
            );

            return await HttpWebServiceClient.DeserializeJsonArrayResponse<TableColumnsOutPOCO>(response);
        }

        public async Task<string> CreateFolder(string parentPath, string name, string? annotation = null)
        {
            return await CreateFolders(new CreateFolderPOCO[]
            {
                new CreateFolderPOCO()
                {
                    ParentPath = parentPath,
                    Name = name,
                    Annotation = annotation
                }
            });
        }

        public async Task<string> CreateFolders(IEnumerable<CreateFolderPOCO> folders)
        {
            IEnumerable<CreateFolderPOCO> foldersSanitized = folders
                .Select(x => new CreateFolderPOCO()
                {
                    Name = x.Name?.Trim('/'),
                    ParentPath = PathExt.Sanitize(x.ParentPath, FolderDelimiter) ?? string.Empty,
                    Annotation = x.Annotation ?? string.Empty
                });

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Post, TdvRestEndpointFeature.Folders, 1)
                .WithContent(foldersSanitized)
            );
        }

        public async Task<string> DropFolder(string folder, bool ifExists = true)
        {
            return await DropFolders(new string[] { folder }, ifExists);
        }

        public async Task<string> DropFolders(IEnumerable<string> folders, bool ifExists = true)
        {
            IEnumerable<string> foldersSanitized = folders
                .Where(folder => !string.IsNullOrWhiteSpace(folder))
                .Select(x => PathExt.Sanitize(x, FolderDelimiter) ?? string.Empty);

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Delete, TdvRestEndpointFeature.Folders, 1)
                .AddQuery(TdvRestEndpointParameter.IfExists, ifExists.ToString().ToLower())
                .WithContent(foldersSanitized)
            );
        }

        public async Task<IEnumerable<string>> DropAnyContainers(IEnumerable<ContainerContentsOutPOCO> paths, bool ifExists = true)
        {
            List<Task<string>> dropFoldersTasks = new ();

            IEnumerable<ContainerContentsOutPOCO> pathsSanitized = paths
                .Where(folderItem => !string.IsNullOrWhiteSpace(folderItem.Path))
                .Select(folderItem => folderItem with
                {
                    Path = PathExt.Sanitize(folderItem.Path, FolderDelimiter)
                });

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.Folder).Any())
            {
                dropFoldersTasks.Add(DropFolders(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.Folder)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedSchema).Any())
            {
                dropFoldersTasks.Add(DropSchemas(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedSchema)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedCatalog).Any())
            {
                dropFoldersTasks.Add(DropCatalogs(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedCatalog)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            return await Task.WhenAll(dropFoldersTasks);
        }

        public async Task<string> DropAnyContainers(IEnumerable<string> paths, TdvResourceTypeEnum folderType, bool ifExists = true)
        {
            if (folderType == TdvResourceTypeEnum.Folder)
                return await DropFolders(paths, ifExists: ifExists);
            else if (folderType == TdvResourceTypeEnum.PublishedCatalog)
                return await DropCatalogs(paths, ifExists: ifExists);
            else if (folderType == TdvResourceTypeEnum.PublishedSchema)
                return await DropSchemas(paths, ifExists: ifExists);
            else
                throw new ArgumentOutOfRangeException(nameof(folderType), folderType.ToString());
        }

        public async Task DropFolderRecursive(string? rootNodePath, bool dropSelf, TdvResourceTypeEnum resourceType = TdvResourceTypeEnum.Folder)
        {
            if (string.IsNullOrWhiteSpace(rootNodePath))
                throw new ArgumentNullException(nameof(rootNodePath));

            IEnumerable<ContainerContentsOutPOCO> folderContents = await RetrieveContainerContents(rootNodePath, resourceType);
            IEnumerable<ContainerContentsOutPOCO> nonemptyFolderContents = folderContents
                .Where(folderItem => !string.IsNullOrWhiteSpace(folderItem.Path));

            IEnumerable<string> viewsToDrop = nonemptyFolderContents
                .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.View)
                .Select(folderItem => PathExt.Sanitize(folderItem.Path, FolderDelimiter) ?? string.Empty);
            IEnumerable<DeleteLinkPOCO> linksToDrop = nonemptyFolderContents
                .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedTableOrView)
                .Select(folderItem => new DeleteLinkPOCO()
                {
                    Path = PathExt.Sanitize(folderItem.Path, FolderDelimiter),
                    IsTable = true
                });
            IEnumerable<ContainerContentsOutPOCO> containersToDrop = nonemptyFolderContents
                .Where(folderItem => folderItem.TdvResourceType is TdvResourceTypeEnum.Folder or TdvResourceTypeEnum.PublishedCatalog or TdvResourceTypeEnum.PublishedSchema);

            IEnumerable<Task> dropTasks = containersToDrop
                .Select(folder => DropFolderRecursive(folder.Path, false))
                // add the single-call mass-deletions
                .Append(DropDataViews(viewsToDrop))
                .Append(DropLinks(linksToDrop));

            await Task.WhenAll(dropTasks);

            await DropAnyContainers(containersToDrop);

            if (dropSelf)
                await DropAnyContainers(new[] { rootNodePath }, resourceType);

            string action = dropSelf ? "deleting" : "purging";
            Console.WriteLine($"Done {action} folder {rootNodePath}"); // 2do! delegate
        }

        public async Task<string> CreateDataViews(IEnumerable<CreateDataViewPOCO> requestBody)
        {
            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Post, TdvRestEndpointFeature.DataView, 1)
                .WithContent(requestBody)
            );
        }

        public async Task<string> CreateDataView(string parentPath, string name, string sql, string? annotation = null, bool ifNotExists = false)
        {
            return await CreateDataViews(new CreateDataViewPOCO[]
            {
                new CreateDataViewPOCO
                {
                    ParentPath = parentPath,
                    Name = name,
                    SQL = sql,
                    IfNotExists = ifNotExists,
                    Annotation = annotation
                }
            });
        }

        public async Task<string> DropDataView(string path, bool ifExists = true)
        {
            return await DropDataViews(new[] { path }, ifExists: ifExists);
        }

        public async Task<string> DropDataViews(IEnumerable<string> paths, bool ifExists = true)
        {
            IEnumerable<string> pathsSanitized = paths
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(x => PathExt.Sanitize(x, FolderDelimiter) ?? string.Empty);

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Delete, TdvRestEndpointFeature.DataView, 1)
                .AddQuery(TdvRestEndpointParameter.IfExists, ifExists.ToString().ToLower())
                .WithContent(pathsSanitized)
            );
        }

        public async Task<string> CreateLinks(IEnumerable<CreateLinkPOCO> links, bool ifNotExists = true, bool? isTableOverride = null, bool? ifNotExistsOverride = null)
        {
            IEnumerable<CreateLinkPOCO> linksSanitized = links
                .Select(link => link with
                {
                    IfNotExists = ifNotExistsOverride ?? link.IfNotExists,
                    IsTable = isTableOverride ?? link.IsTable
                });

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Post, TdvRestEndpointFeature.Link, 1)
                .AddQuery(TdvRestEndpointParameter.IfNotExists, ifNotExists.ToString().ToLower())
                .WithContent(linksSanitized)
            );
        }

        public async Task<string> DropLinks(IEnumerable<DeleteLinkPOCO> links, bool ifExists = true, bool? isTableOverride = null)
        {
            IEnumerable<DeleteLinkPOCO> linksSanitized = links
                .Select(link => link with
                {
                    IsTable = isTableOverride ?? link.IsTable
                });

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Delete, TdvRestEndpointFeature.Link, 1)
                .AddQuery(TdvRestEndpointParameter.IfExists, ifExists.ToString().ToLower())
                .WithContent(linksSanitized)
            );
        }

        public async Task<string> CreateSchemas(IEnumerable<CreateSchemaPOCO> schemas, bool? ifNotExistsOverride = null)
        {
            IEnumerable<CreateSchemaPOCO> schemasSanitized = schemas
                .Where(schema => !string.IsNullOrWhiteSpace(schema.Path))
                .Select(schema => schema with
                {
                    Path = PathExt.Sanitize(schema.Path, FolderDelimiter),
                    IfNotExists = ifNotExistsOverride ?? schema.IfNotExists
                });

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Post, TdvRestEndpointFeature.Schema, 1)
                .AddResourceFolder("virtual")
                .WithContent(schemasSanitized)
            );
        }

        public async Task<string> DropSchemas(IEnumerable<string> schemas, bool ifExists = true)
        {
            IEnumerable<string> schemasSanitized = schemas
                .Where(schema => !string.IsNullOrWhiteSpace(schema))
                .Select(schema => PathExt.Sanitize(schema, FolderDelimiter) ?? string.Empty);

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Delete, TdvRestEndpointFeature.Schema, 1)
                .AddResourceFolder("virtual")
                .AddQuery(TdvRestEndpointParameter.IfExists, ifExists.ToString().ToLower())
                .WithContent(schemasSanitized)
            );
        }

        public async Task<string> CreateCatalogs(IEnumerable<CreateCatalogPOCO> catalogs, bool? ifNotExistsOverride = null)
        {
            IEnumerable<CreateCatalogPOCO> catalogsSanitized = catalogs
                .Where(catalog => !string.IsNullOrWhiteSpace(catalog.Path))
                .Select(catalog => catalog with
                {
                    Path = PathExt.Sanitize(catalog.Path, FolderDelimiter),
                    NewPath = PathExt.Sanitize(catalog.Path, FolderDelimiter),
                    IfNotExists = ifNotExistsOverride ?? catalog.IfNotExists
                });

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Post, TdvRestEndpointFeature.Catalog, 1)
                .AddResourceFolder("virtual")
                .WithContent(catalogsSanitized)
            );
        }

        public async Task<string> DropCatalogs(IEnumerable<string> catalogs, bool ifExists = true)
        {
            IEnumerable<string> catalogsSanitized = catalogs
                .Where(catalog => !string.IsNullOrWhiteSpace(catalog))
                .Select(catalog => PathExt.Sanitize(catalog, FolderDelimiter) ?? string.Empty);

            return await _httpRestClient.CallEndpointString(new TdvRestWsEndpoint(HttpMethod.Delete, TdvRestEndpointFeature.Catalog, 1)
                .AddResourceFolder("virtual")
                .AddQuery(TdvRestEndpointParameter.IfExists, ifExists.ToString().ToLower())
                .WithContent(catalogsSanitized)
            );
        }

        public async Task<string> UpdateResourcePrivileges(
            IEnumerable<SoapPrivilegeEntryPOCO> privEntries,
            bool recursiveUpdate = false,
            SoapUpdateResourcePrivileges_Mode updateMode = SoapUpdateResourcePrivileges_Mode.OverwriteAppend
        )
        {
            SoapUpdateResourcePrivilegesPOCO input = new ()
            {
                UpdateRecursively = recursiveUpdate,
                Mode = updateMode,
                PrivilegeEntries = privEntries.ToArray()
            };

            return await _httpRestClient.CallEndpointString(new TdvSoapWsEndpoint<SoapUpdateResourcePrivilegesPOCO>("updateResourcePrivileges", input)
                .AddResourceFolder("system")
                .AddResourceFolder("admin")
                .AddResourceFolder("resource")
            );
        }
    }
}
