namespace NoP77svk.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using NoP77svk.Exceptions;

    public static class EnumerableChunkingExt
    {
        public static IEnumerable<ChunkOf<TElement>> ChunkByMeasure<TElement>(
            this IEnumerable<TElement> self,
            Func<TElement, int> calculateElementMeasure,
            int chunkThresholdMeasure
        )
        {
            if (chunkThresholdMeasure <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkThresholdMeasure), chunkThresholdMeasure, "Chunk threshold measure must be positive integer");

            ChunkOf<TElement> result = new ChunkOf<TElement>()
            {
                Chunk = null,
                TotalChunkMeasure = 0
            };
            int elementIndex = 0;

            foreach (TElement element in self)
            {
                elementIndex++;

                int elementMeasure = calculateElementMeasure(element);
                if (elementMeasure < 0)
                    throw new ValueConstraintViolatedException($"Negative calculated element[{elementIndex}] measure");

                if (result.TotalChunkMeasure + elementMeasure > chunkThresholdMeasure)
                {
                    if (result.Chunk == null)
                        throw new ValueConstraintViolatedException($"Element[{elementIndex}] measure {elementMeasure} over chunking threshold {chunkThresholdMeasure}");

                    yield return result;

                    result.Chunk = null;
                    result.TotalChunkMeasure = 0;
                }

                if (result.Chunk == null)
                {
                    result = new ChunkOf<TElement>()
                    {
                        Chunk = new List<TElement>(),
                        TotalChunkMeasure = 0
                    };
                }

                result.Chunk.Add(element);
                result.TotalChunkMeasure += elementMeasure;
            }

            if (result.Chunk != null)
                yield return result;
        }

        public static IEnumerable<ChunkOf<TElement>> ChunkByCount<TElement>(
            this IEnumerable<TElement> self,
            int chunkThresholdElementCount
        )
        {
            return self.ChunkByMeasure(_ => 1, chunkThresholdElementCount);
        }

        public static async IAsyncEnumerable<ChunkOf<TElement>> ChunkByMeasure<TElement>(
            this IAsyncEnumerable<TElement> self,
            Func<TElement, int> calculateElementMeasure,
            int chunkThresholdMeasure
        )
        {
            if (chunkThresholdMeasure <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkThresholdMeasure), chunkThresholdMeasure, "Chunk threshold measure must be positive integer");

            ChunkOf<TElement> result = new ChunkOf<TElement>()
            {
                Chunk = null,
                TotalChunkMeasure = 0
            };
            int elementIndex = 0;

            await foreach (TElement element in self)
            {
                elementIndex++;

                int elementMeasure = calculateElementMeasure(element);
                if (elementMeasure < 0)
                    throw new ValueConstraintViolatedException($"Negative calculated element[{elementIndex}] measure");

                if (result.TotalChunkMeasure + elementMeasure > chunkThresholdMeasure)
                {
                    if (result.Chunk == null)
                        throw new ValueConstraintViolatedException($"Element[{elementIndex}] measure {elementMeasure} over chunking threshold {chunkThresholdMeasure}");

                    yield return result;

                    result.Chunk = null;
                    result.TotalChunkMeasure = 0;
                }

                if (result.Chunk == null)
                {
                    result = new ChunkOf<TElement>()
                    {
                        Chunk = new List<TElement>(),
                        TotalChunkMeasure = 0
                    };
                }

                result.Chunk.Add(element);
                result.TotalChunkMeasure += elementMeasure;
            }

            if (result.Chunk != null)
                yield return result;
        }

        public static async IAsyncEnumerable<ChunkOf<TElement>> ChunkByCount<TElement>(
            this IAsyncEnumerable<TElement> self,
            int chunkThresholdElementCount
        )
        {
            await foreach (ChunkOf<TElement> element in self.ChunkByMeasure(_ => 1, chunkThresholdElementCount))
                yield return element;
        }
    }
}
