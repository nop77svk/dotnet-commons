﻿namespace NoP77svk.API.TibcoDV
{
    using System.Text.Json.Serialization;

    public record ContainerContentsOutPOCO
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        [JsonPropertyName("subtype")]
        public string? SubType { get; set; }
        public string? TargetType { get; set; }
        public object? ImpactMessage { get; set; }
        public int? ChildCount { get; set; }

        public TdvResourceTypeEnum TdvResourceType => TdvWebServiceClient.DetermineResourceType(this);
    }
}
