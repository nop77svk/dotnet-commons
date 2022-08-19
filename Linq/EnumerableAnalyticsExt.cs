namespace NoP77svk.Linq
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableAnalyticsExt
    {
        public static IEnumerable<(TElement Current, TElement? Lagged)> Lag<TElement>(this IEnumerable<TElement> collection, Func<TElement, TElement, bool> areInTheSamePartition, int lagSize = 1)
            where TElement : class
        {
            if (lagSize < 0)
                throw new ArgumentOutOfRangeException(nameof(lagSize), lagSize, "Non-negative integer expected");

            Queue<TElement> laggedElements = new Queue<TElement>(lagSize);
            TElement? previousElement = null;
            foreach (TElement element in collection)
            {
                if (previousElement == null)
                    laggedElements.Clear();
                else if (!areInTheSamePartition(element, previousElement))
                    laggedElements.Clear();

                TElement? resultLaggedElement;
                if (laggedElements.Count == lagSize)
                    resultLaggedElement = laggedElements.Dequeue();
                else
                    resultLaggedElement = null;

                yield return (element, resultLaggedElement);

                laggedElements.Enqueue(element);
                previousElement = element;
            }
        }

        public static IEnumerable<(TElement Current, TElement? Lagged)> Lag<TElement>(this IEnumerable<TElement> collection, int lagSize = 1)
            where TElement : class
        {
            return collection.Lag((current, lagged) => true, lagSize);
        }

        public static IEnumerable<(TElement Current, TElement? Lagged)> Lag<TElement, TPartitionKey>(this IEnumerable<TElement> collection, Func<TElement, TPartitionKey> getPartitionKey, int lagSize = 1)
            where TElement : class
            where TPartitionKey : IEquatable<TPartitionKey>
        {
            return collection.Lag((element, previousElement) => getPartitionKey(element).Equals(getPartitionKey(previousElement)));
        }

        public static IEnumerable<(int RunId, TElement Element)> RecognizeElementRuns<TElement>(this IEnumerable<TElement> collection, Func<TElement, TElement?, bool> areInTheSameRun)
            where TElement : class
        {
            int runId = 0;
            foreach ((TElement current, TElement? lagged) in collection.Lag())
            {
                bool newRunStartsHere = runId == 0
                    || !areInTheSameRun(current, lagged);

                if (newRunStartsHere)
                    runId++;

                yield return (runId, current);
            }
        }
    }
}
