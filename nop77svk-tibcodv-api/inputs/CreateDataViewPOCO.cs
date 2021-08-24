namespace NoP77svk.API.TibcoDV
{
    public record CreateDataViewPOCO : CreateAnyObjectPOCO
    {
        public string SQL { get; init; } = string.Empty;
    }
}
