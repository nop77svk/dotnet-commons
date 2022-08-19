namespace NoP77svk.Linq
{
    using System.Collections.Generic;

    public struct ChunkOf<TElement>
    {
        public List<TElement>? Chunk;
        public int TotalChunkMeasure;
    }
}
