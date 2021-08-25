namespace NoP77svk.Collections.Generic
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class IAsyncEnumerableExt
    {
        public static async Task<List<TElement>> ToList<TElement>(this IAsyncEnumerable<TElement> self)
        {
            List<TElement> result = new List<TElement>();

            await foreach (TElement item in self)
                result.Add(item);

            return result;
        }
    }
}
