namespace NoP77svk.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class EnumerableExt
    {
        public static IEnumerable<ValueTuple<TLeft, TRight>> CrossProduct<TLeft, TRight>(
            this IEnumerable<TLeft> leftDataset,
            IEnumerable<TRight> rightDataset
        )
        {
            foreach (TLeft leftRecord in leftDataset)
            {
                foreach (TRight rightRecord in rightDataset)
                    yield return new ValueTuple<TLeft, TRight>(leftRecord, rightRecord);
            }
        }

        public static IEnumerable<TResult> CrossProduct<TLeft, TRight, TResult>(
            this IEnumerable<TLeft> leftDataset,
            IEnumerable<TRight> rightDataset,
            Func<TLeft, TRight, TResult> resultSelector
        )
        {
            foreach (ValueTuple<TLeft, TRight> joinedRecord in leftDataset.CrossProduct(rightDataset))
                yield return resultSelector(joinedRecord.Item1, joinedRecord.Item2);
        }

        public static IEnumerable<ValueTuple<TElementToMatch, TElementWithRegexp>> JoinByRegexpMatch<TElementToMatch, TElementWithRegexp>(
            this IEnumerable<TElementToMatch> self,
            Func<TElementToMatch, string?> valueSelector,
            IEnumerable<TElementWithRegexp> regexps,
            Func<TElementWithRegexp, Regex?> regexpSelector
        )
        {
            foreach (ValueTuple<TElementToMatch, TElementWithRegexp> crossRecord in self.CrossProduct(regexps))
            {
                string? valueToMatch = valueSelector(crossRecord.Item1);
                if (valueToMatch != null)
                {
                    Regex? regex = regexpSelector(crossRecord.Item2);
                    if (regex != null)
                    {
                        if (regex.IsMatch(valueToMatch))
                            yield return crossRecord;
                    }
                }
            }
        }

        public static IEnumerable<TResult> Unnest<TElement, TSubElement, TResult>(
            this IEnumerable<TElement> self,
            Func<TElement, IEnumerable<TSubElement>> retrieveNestedCollection,
            Func<TElement, TSubElement, TResult> resultSelector
        )
        {
            foreach (TElement element in self)
            {
                IEnumerable<TSubElement> nestedCollection = retrieveNestedCollection(element);
                foreach (TSubElement subElement in nestedCollection)
                    yield return resultSelector(element, subElement);
            }
        }
    }
}
