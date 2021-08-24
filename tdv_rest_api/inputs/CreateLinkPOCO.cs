namespace NoP77svk.API.TibcoDV
{
    using System.Text.Json.Serialization;

    public record CreateLinkPOCO
    {
        [JsonPropertyName("path")]
        public string? PublishedLinkPath { get; init; }

        public bool IsTable { get; init; } = true;

        [JsonPropertyName("targetPath")]
        public string? SourceObjectPath { get; init; }

        public string? Annotation { get; init; }

        public bool IfNotExists { get; init; } = false;
    }
}
