namespace NoP77svk.API.TibcoDV
{
    public record CreateSchemaPOCO
    {
        public string? Path { get; init; }
        public string? Annotation { get; init; }
        public bool IfNotExists { get; init; } = true;
    }
}
