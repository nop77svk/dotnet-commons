namespace TelemetryLogsExtractor.Infrastructure;

public static class EnumerablePivotingExtensions
{
    extension<TElement>(IEnumerable<TElement> values)
    {
        public IEnumerable<KeyValuePair<TGroupKey, IEnumerable<KeyValuePair<TPivotKey, TPivotElement>>>> Pivot<TGroupKey, TPivotKey, TPivotElement>(
            Func<TElement, TGroupKey> groupBy,
            Func<TElement, TPivotKey> pivotFor,
            Func<TGroupKey, TPivotKey, IEnumerable<TElement>, TPivotElement> pivotAggregateValueSelector)
        {
            var result = values
                .GroupBy(
                    row => groupBy(row),
                    resultSelector: (groupKey, groupKeyGroupedCollection)
                        => new KeyValuePair<TGroupKey, IEnumerable<KeyValuePair<TPivotKey, TPivotElement>>>(
                            groupKey,
                            groupKeyGroupedCollection
                                .GroupBy(
                                    row => pivotFor(row),
                                    resultSelector: (pivotKey, pivotKeyGroupedCollection)
                                        => new KeyValuePair<TPivotKey, TPivotElement>(
                                            pivotKey,
                                            pivotAggregateValueSelector(groupKey, pivotKey, pivotKeyGroupedCollection)
                                        )
                                )
                        )
                );

            return result;
        }

        public IEnumerable<KeyValuePair<TGroupKey, IEnumerable<KeyValuePair<TPivotKey, IEnumerable<TElement>>>>> Pivot<TGroupKey, TPivotKey>(
            Func<TElement, TGroupKey> groupKeySelector,
            Func<TElement, TPivotKey> pivotKeySelector)
            => values.Pivot(
                groupBy: groupKeySelector,
                pivotFor: pivotKeySelector,
                pivotAggregateValueSelector: (_, _, elements) => elements
            );
    }
}
