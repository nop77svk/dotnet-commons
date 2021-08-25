namespace NoP77svk.API.TibcoDV
{
    public abstract record CreateAnyObjectPOCO
    {
        public string ParentPath { get; init; } = "/";
        public string? Name { get; init; }
        public bool IfNotExists { get; init; } = true;
        public string? Annotation { get; init; }
    }
}
