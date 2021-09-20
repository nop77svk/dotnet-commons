namespace NoP77svk.API.TibcoDV
{
    public record TdvRest_DeleteLink
    {
        public string? Path { get; init; }

        public bool IsTable { get; init; } = true;
    }
}
