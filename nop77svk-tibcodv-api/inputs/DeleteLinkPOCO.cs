namespace NoP77svk.API.TibcoDV
{
    public record DeleteLinkPOCO
    {
        public string? Path { get; init; }

        public bool IsTable { get; init; } = true;
    }
}
