namespace NoP77svk.Linq
{
    using System.Collections.Generic;

    public static partial class EnumerableChunkingExt
    {
        public struct ChunkOf<TElement>
        {
            public List<TElement>? Chunk;
            public int TotalChunkMeasure;
        }
    }
}
