namespace NoP77svk.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using NoP77svk.Exceptions;

    public static class IEnumerableChunkExt
    {
        public static IEnumerable<List<TElement>> ChunkByMeasure<TElement>(
            this IEnumerable<TElement> self,
            Func<TElement, int> calculateElementMeasure,
            int chunkThresholdMeasure
        )
        {
            if (chunkThresholdMeasure <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkThresholdMeasure), chunkThresholdMeasure, "Chunk threshold measure must be positive integer");

            List<TElement>? result = null;
            int totalElementMeasure = 0;
            int elementIndex = 0;

            foreach (TElement element in self)
            {
                elementIndex++;

                int elementMeasure = calculateElementMeasure(element);
                if (elementMeasure < 0)
                    throw new ValueConstraintViolatedException($"Negative calculated element[{elementIndex}] measure");

                if (totalElementMeasure + elementMeasure > chunkThresholdMeasure)
                {
                    if (result == null)
                        throw new ValueConstraintViolatedException($"Element[{elementIndex}] measure {elementMeasure} over chunking threshold {chunkThresholdMeasure}");

                    yield return result;
                    result = null;
                }

                if (result == null)
                    result = new List<TElement>();

                result.Add(element);
            }

            if (result != null)
                yield return result;
        }

        public static IEnumerable<List<TElement>> ChunkByCount<TElement>(
            this IEnumerable<TElement> self,
            int chunkThresholdElementCount
        )
        {
            return self.ChunkByMeasure(_ => 1, chunkThresholdElementCount);
        }
    }
}
