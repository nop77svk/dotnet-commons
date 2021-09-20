namespace NoP77svk.API.TibcoDV
{
    public record TdvRest_CreateDataView
        : TdvRest_CreateAnyObject
    {
        public string SQL { get; init; } = string.Empty;
    }
}
