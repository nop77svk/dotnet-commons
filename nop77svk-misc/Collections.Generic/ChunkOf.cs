namespace NoP77svk.Collections.Generic
{
    using System.Collections.Generic;

    public struct ChunkOf<TElement>
    {
        public List<TElement>? Chunk;
        public int TotalChunkMeasure;
    }
}
