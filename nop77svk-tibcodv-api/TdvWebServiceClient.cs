namespace NoP77svk.API.TibcoDV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NoP77svk.Collections.Generic;
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

        public TdvWebServiceClient(HttpWebServiceClient httpWsClient, int apiVersion = 1)
        {
            HttpWsClient = httpWsClient;
            ApiVersion = apiVersion;
        }

        public int ApiVersion { get; init; }

        public HttpWebServiceClient HttpWsClient { get; private set; }

        public static TdvResourceTypeEnum CalcResourceType(TdvRest_ContainerContents resource)
        {
            try
            {
                return TdvResourceType.CalcResourceType(resource.Type, resource.SubType, resource.TargetType);
            }
            catch (Exception e)
            {
                throw new ArgumentOutOfRangeException($"Error while determining type of resource \"{resource.Path}\"", e);
            }
        }

        public async IAsyncEnumerable<TdvSoap_GetResourceResponse> GetResourceInfo(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            IAsyncEnumerable<TdvSoap_GetResourceResponse> resInfoAll = HttpWsClient.EndpointGetSoap<TdvSoap_GetResourceResponse>(
                new TdvSoapWsEndpoint<TdvSoap_GetResource>(
                    "getResource",
                    new TdvSoap_GetResource()
                    {
                        Path = path,
                        Type = null,
                        Detail = TdvSoap_GetResource_DetailEnum.Simple
                    })
                        .AddResourceFolder("system")
                        .AddResourceFolder("admin")
                        .AddResourceFolder("resource")
            );

            await foreach (TdvSoap_GetResourceResponse resInfoBody in resInfoAll)
                yield return resInfoBody;
        }

        public async Task<List<TdvRest_ContainerContents>> RetrieveContainerContents(string? path, TdvResourceTypeEnum resourceType)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            (string jsonResourceType, _) = TdvResourceType.CalcWsResourceTypes(resourceType);

            return await HttpWsClient.EndpointGetJson<List<TdvRest_ContainerContents>>(TdvRestWsEndpoint.ResourceApi(HttpMethod.Get)
                .AddResourceFolder("children")
                .AddQuery("path", path)
                .AddQuery("type", jsonResourceType.ToLower())
            );
        }

        public async IAsyncEnumerable<TdvRest_ContainerContents> RetrieveContainerContentsRecursive(string? path, TdvResourceTypeEnum resourceType)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            List<Task<List<TdvRest_ContainerContents>>> subfolderReaders = new ();
            subfolderReaders.Add(RetrieveContainerContents(path, resourceType));

            while (subfolderReaders.Any())
            {
                Task<List<TdvRest_ContainerContents>> finishedSubfolderReader = await Task.WhenAny(subfolderReaders);
                subfolderReaders.Remove(finishedSubfolderReader);

                foreach (TdvRest_ContainerContents folderItem in finishedSubfolderReader.Result)
                {
                    if (folderItem.TdvResourceType is TdvResourceTypeEnum.Folder
                        or TdvResourceTypeEnum.PublishedCatalog
                        or TdvResourceTypeEnum.PublishedSchema
                        or TdvResourceTypeEnum.DataSourceCompositeWebService
                        or TdvResourceTypeEnum.DataSourceRelational)
                    {
                        subfolderReaders.Add(RetrieveContainerContents(folderItem.Path, folderItem.TdvResourceType));
                    }

                    yield return folderItem;
                }
            }
        }

        public async Task<ICollection<TdvRest_TableColumns>> RetrieveTableColumns(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            return await HttpWsClient.EndpointGetJson<List<TdvRest_TableColumns>>(TdvRestWsEndpoint.ResourceApi(HttpMethod.Get)
                .AddResourceFolder("table")
                .AddResourceFolder("columns")
                .AddQuery("path", path)
                .AddQuery("type", "table")
            );
        }

        public async Task<string> CreateFolder(string parentPath, string name, string? annotation = null, bool ifNotExists = true)
        {
            return await CreateFolders(new TdvRest_CreateFolder[]
            {
                new TdvRest_CreateFolder()
                {
                    ParentPath = parentPath,
                    Name = name,
                    Annotation = annotation,
                    IfNotExists = ifNotExists
                }
            });
        }

        public async Task<string> CreateFolders(IEnumerable<TdvRest_CreateFolder> folders)
        {
            IEnumerable<TdvRest_CreateFolder> foldersSanitized = folders
                .Select(x => x with
                {
                    Name = x.Name?.Trim('/'),
                    ParentPath = PathExt.Sanitize(x.ParentPath, FolderDelimiter) ?? string.Empty,
                    Annotation = x.Annotation ?? string.Empty
                });

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.FoldersApi(HttpMethod.Post)
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

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.FoldersApi(HttpMethod.Delete)
                .AddTdvQuery(TdvRestEndpointParameterConst.IfExists, ifExists)
                .WithContent(foldersSanitized)
            );
        }

        public async Task DropAnyContainers(IEnumerable<TdvRest_ContainerContents> paths, bool ifExists = true)
        {
            List<Task<string>> dropContainerTasks = new ();

            IEnumerable<TdvRest_ContainerContents> pathsSanitized = paths
                .Where(folderItem => !string.IsNullOrWhiteSpace(folderItem.Path))
                .Select(folderItem => folderItem with
                {
                    Path = PathExt.Sanitize(folderItem.Path, FolderDelimiter)
                });

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.Folder).Any())
            {
                dropContainerTasks.Add(DropFolders(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.Folder)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedSchema).Any())
            {
                dropContainerTasks.Add(DropSchemas(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedSchema)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            if (pathsSanitized.Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedCatalog).Any())
            {
                dropContainerTasks.Add(DropCatalogs(
                    pathsSanitized
                        .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedCatalog)
                        .Select(folderItem => folderItem.Path ?? "???"),
                    ifExists: ifExists
                ));
            }

            await Task.WhenAll(dropContainerTasks);
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

        public async Task DropFolderRecursive(
            string? rootNodePath,
            bool dropSelf,
            TdvResourceTypeEnum resourceType = TdvResourceTypeEnum.Folder,
            Action<string>? doneFeedbackCallback = null
        )
        {
            if (string.IsNullOrWhiteSpace(rootNodePath))
                throw new ArgumentNullException(nameof(rootNodePath));

            IEnumerable<TdvRest_ContainerContents> folderContents = await RetrieveContainerContents(rootNodePath, resourceType);
            IEnumerable<TdvRest_ContainerContents> nonemptyFolderContents = folderContents
                .Where(folderItem => !string.IsNullOrWhiteSpace(folderItem.Path));

            IEnumerable<string> viewsToDrop = nonemptyFolderContents
                .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.View)
                .Select(folderItem => PathExt.Sanitize(folderItem.Path, FolderDelimiter) ?? string.Empty);
            IEnumerable<TdvRest_DeleteLink> linksToDrop = nonemptyFolderContents
                .Where(folderItem => folderItem.TdvResourceType == TdvResourceTypeEnum.PublishedTableOrView)
                .Select(folderItem => new TdvRest_DeleteLink()
                {
                    Path = PathExt.Sanitize(folderItem.Path, FolderDelimiter),
                    IsTable = true
                });
            IEnumerable<TdvRest_ContainerContents> containersToDrop = nonemptyFolderContents
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

            doneFeedbackCallback?.Invoke(rootNodePath);
        }

        public async Task<string> CreateDataViews(IEnumerable<TdvRest_CreateDataView> requestBody)
        {
            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.DataViewApi(HttpMethod.Post)
                .WithContent(requestBody)
            );
        }

        public async Task<string> CreateDataView(string parentPath, string name, string sql, string? annotation = null, bool ifNotExists = false)
        {
            return await CreateDataViews(new TdvRest_CreateDataView[]
            {
                new TdvRest_CreateDataView
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

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.DataViewApi(HttpMethod.Delete)
                .AddTdvQuery(TdvRestEndpointParameterConst.IfExists, ifExists)
                .WithContent(pathsSanitized)
            );
        }

        public async Task<string> CreateLinks(IEnumerable<TdvRest_CreateLink> links, bool ifNotExists = true, bool? isTableOverride = null, bool? ifNotExistsOverride = null)
        {
            IEnumerable<TdvRest_CreateLink> linksSanitized = links
                .Select(link => link with
                {
                    IfNotExists = ifNotExistsOverride ?? link.IfNotExists,
                    IsTable = isTableOverride ?? link.IsTable
                });

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.LinkApi(HttpMethod.Post)
                .AddTdvQuery(TdvRestEndpointParameterConst.IfNotExists, ifNotExists)
                .WithContent(linksSanitized)
            );
        }

        public async Task<string> DropLinks(IEnumerable<TdvRest_DeleteLink> links, bool ifExists = true, bool? isTableOverride = null)
        {
            IEnumerable<TdvRest_DeleteLink> linksSanitized = links
                .Select(link => link with
                {
                    IsTable = isTableOverride ?? link.IsTable
                });

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.LinkApi(HttpMethod.Delete)
                .AddTdvQuery(TdvRestEndpointParameterConst.IfExists, ifExists)
                .WithContent(linksSanitized)
            );
        }

        public async Task<string> CreateSchemas(IEnumerable<TdvRest_CreateSchema> schemas, bool? ifNotExistsOverride = null)
        {
            IEnumerable<TdvRest_CreateSchema> schemasSanitized = schemas
                .Where(schema => !string.IsNullOrWhiteSpace(schema.Path))
                .Select(schema => schema with
                {
                    Path = PathExt.Sanitize(schema.Path, FolderDelimiter),
                    IfNotExists = ifNotExistsOverride ?? schema.IfNotExists
                });

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.SchemaApi(HttpMethod.Post)
                .AddResourceFolder("virtual")
                .WithContent(schemasSanitized)
            );
        }

        public async Task<string> DropSchemas(IEnumerable<string> schemas, bool ifExists = true)
        {
            IEnumerable<string> schemasSanitized = schemas
                .Where(schema => !string.IsNullOrWhiteSpace(schema))
                .Select(schema => PathExt.Sanitize(schema, FolderDelimiter) ?? string.Empty);

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.SchemaApi(HttpMethod.Delete)
                .AddResourceFolder("virtual")
                .AddTdvQuery(TdvRestEndpointParameterConst.IfExists, ifExists)
                .WithContent(schemasSanitized)
            );
        }

        public async Task<string> CreateCatalogs(IEnumerable<TdvRest_CreateCatalog> catalogs, bool? ifNotExistsOverride = null)
        {
            IEnumerable<TdvRest_CreateCatalog> catalogsSanitized = catalogs
                .Where(catalog => !string.IsNullOrWhiteSpace(catalog.Path))
                .Select(catalog => catalog with
                {
                    Path = PathExt.Sanitize(catalog.Path, FolderDelimiter),
                    NewPath = PathExt.Sanitize(catalog.Path, FolderDelimiter),
                    IfNotExists = ifNotExistsOverride ?? catalog.IfNotExists
                });

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.CatalogApi(HttpMethod.Post)
                .AddResourceFolder("virtual")
                .WithContent(catalogsSanitized)
            );
        }

        public async Task<string> DropCatalogs(IEnumerable<string> catalogs, bool ifExists = true)
        {
            IEnumerable<string> catalogsSanitized = catalogs
                .Where(catalog => !string.IsNullOrWhiteSpace(catalog))
                .Select(catalog => PathExt.Sanitize(catalog, FolderDelimiter) ?? string.Empty);

            return await HttpWsClient.EndpointGetString(TdvRestWsEndpoint.CatalogApi(HttpMethod.Delete)
                .AddResourceFolder("virtual")
                .AddTdvQuery(TdvRestEndpointParameterConst.IfExists, ifExists)
                .WithContent(catalogsSanitized)
            );
        }

        public async Task<string> UpdateResourcePrivileges(
            IEnumerable<TdvSoap_PrivilegeEntry> privEntries,
            bool recursiveUpdate = false,
            TdvSoap_UpdateResourcePrivileges_ModeEnum updateMode = TdvSoap_UpdateResourcePrivileges_ModeEnum.OverwriteAppend
        )
        {
            TdvSoap_UpdateResourcePrivileges input = new ()
            {
                UpdateRecursively = recursiveUpdate,
                Mode = updateMode,
                PrivilegeEntries = privEntries.ToArray()
            };

            IAsyncEnumerable<TdvSoap_UpdateResourcePrivilegesResponse> result = HttpWsClient.EndpointGetSoap<TdvSoap_UpdateResourcePrivilegesResponse>(new TdvSoapWsEndpoint<TdvSoap_UpdateResourcePrivileges>("updateResourcePrivileges", input)
                .AddResourceFolder("system")
                .AddResourceFolder("admin")
                .AddResourceFolder("resource")
            );

            return string.Join(Environment.NewLine, (await result.ToList()).Select(x => x.Body));
        }
    }
}
