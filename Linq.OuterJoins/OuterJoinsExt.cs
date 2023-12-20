namespace NoP77svk.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class OuterJoinsExt
    {
        public static IEnumerable<TResult> LeftOuterJoin<TOuterRow, TInnerRow, TKey, TResult>(
            this IEnumerable<TOuterRow> outerTable,
            IEnumerable<TInnerRow> innerTable,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TInnerRow, TKey> innerKeySelector,
            Func<TOuterRow, TInnerRow, TResult> resultSelector)
            where TKey : IEquatable<TKey>
        {
            return
                from left in outerTable
                join right in innerTable
                    on outerKeySelector(left) equals innerKeySelector(right)
                    into temp
                from right in temp.DefaultIfEmpty()
                select resultSelector(left, right);
        }

        public static IEnumerable<TResult> RightOuterJoin<TInnerRow, TOuterRow, TKey, TResult>(
            this IEnumerable<TInnerRow> innerTable,
            IEnumerable<TOuterRow> outerTable,
            Func<TInnerRow, TKey> innerKeySelector,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TInnerRow, TOuterRow, TResult> resultSelector)
            where TKey : IEquatable<TKey>
        {
            return
                from right in outerTable
                join left in innerTable
                    on outerKeySelector(right) equals innerKeySelector(left)
                    into temp
                from left in temp.DefaultIfEmpty()
                select resultSelector(left, right);
        }

        public static IEnumerable<TResult> FullOuterJoinDistinct<TOuterRow, TInnerRow, TKey, TResult>(
            this IEnumerable<TOuterRow> outerTable,
            IEnumerable<TInnerRow> innerTable,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TInnerRow, TKey> innerKeySelector,
            Func<TOuterRow, TInnerRow, TResult> resultSelector)
            where TKey : IEquatable<TKey>
        {
            return outerTable
                .LeftOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector)
                .Union(outerTable
                    .RightOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector));
        }

        public static IEnumerable<TOuterRow> SemiJoin<TOuterRow, TSemiRow, TKey>(
            this IEnumerable<TOuterRow> outerTable,
            IEnumerable<TSemiRow> semiJoinedTable,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TSemiRow, TKey> semiJoinKeySelector)
            where TKey : IEquatable<TKey>
        {
            // var hashLK = new HashSet<TKey>(from l in semiJoinedTable select semiJoinKeySelector(l));
            var hashLK = new HashSet<TKey>(semiJoinedTable.Select(l => semiJoinKeySelector(l)));
            return outerTable
                .Where(r => hashLK.Contains(outerKeySelector(r)));
        }

        public static IEnumerable<TOuterRow> AntiJoin<TOuterRow, TAntiRow, TKey>(
            this IEnumerable<TOuterRow> outerTable,
            IEnumerable<TAntiRow> antiJoinedTable,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TAntiRow, TKey> antiJoinKeySelector)
            where TKey : IEquatable<TKey>
        {
            // var hashLK = new HashSet<TKey>(from l in antiJoinedTable select antiJoinKeySelector(l));
            var hashLK = new HashSet<TKey>(antiJoinedTable.Select(l => antiJoinKeySelector(l)));
            return outerTable
                .Where(r => !hashLK.Contains(outerKeySelector(r)));
        }

        public static IEnumerable<TResult> FullOuterJoin<TOuterRow, TInnerRow, TKey, TResult>(
            this IEnumerable<TOuterRow> outerTable,
            IEnumerable<TInnerRow> innerTable,
            Func<TOuterRow, TKey> outerKeySelector,
            Func<TInnerRow, TKey> innerKeySelector,
            Func<TOuterRow, TInnerRow, TResult> resultSelector)
            where TOuterRow : class
            where TKey : IEquatable<TKey>
        {
            return outerTable
                .LeftOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector)
                .Concat(outerTable
                    .AntiJoin(innerTable, outerKeySelector, innerKeySelector)
                    .Select(r => resultSelector(r, default)));
        }

        public static IQueryable<TResult> LeftOuterJoin<TOuterRow, TInnerRow, TKey, TResult>(
            this IQueryable<TOuterRow> outerTable,
            IQueryable<TInnerRow> innerTable,
            Expression<Func<TOuterRow, TKey>> outerKeySelector,
            Expression<Func<TInnerRow, TKey>> innerKeySelector,
            Expression<Func<TOuterRow, TInnerRow, TResult>> resultSelector)
            where TKey : IEquatable<TKey>
        {
            var sampleAnonLR = new { left = default(TOuterRow), right = default(IEnumerable<TInnerRow>) };
            var parmP = Expression.Parameter(sampleAnonLR.GetType(), "p");
            var parmC = Expression.Parameter(typeof(TInnerRow), "c");
            var argLeft = Expression.PropertyOrField(parmP, "left");
            var newleftrs = CastSMBody(Expression.Lambda(Expression.Invoke(resultSelector, argLeft, parmC), parmP, parmC), sampleAnonLR, default(TInnerRow), default(TResult));

            return outerTable
                .AsQueryable()
                .GroupJoin(innerTable, outerKeySelector, innerKeySelector, (left, right) => new { left, right })
                .SelectMany(r => r.right.DefaultIfEmpty(), newleftrs);
        }

        public static IQueryable<TResult> RightOuterJoin<TInnerRow, TOuterRow, TKey, TResult>(
            this IQueryable<TInnerRow> innerTable,
            IQueryable<TOuterRow> outerTable,
            Expression<Func<TInnerRow, TKey>> innerKeySelector,
            Expression<Func<TOuterRow, TKey>> outerKeySelector,
            Expression<Func<TInnerRow, TOuterRow, TResult>> resultSelector)
            where TKey : IEquatable<TKey>
        {
            var sampleAnonLR = new { leftg = default(IEnumerable<TInnerRow>), right = default(TOuterRow) };
            var parmP = Expression.Parameter(sampleAnonLR.GetType(), "p");
            var parmC = Expression.Parameter(typeof(TInnerRow), "c");
            var argRight = Expression.PropertyOrField(parmP, "right");
            var newrightrs = CastSMBody(Expression.Lambda(Expression.Invoke(resultSelector, parmC, argRight), parmP, parmC), sampleAnonLR, default(TInnerRow), default(TResult));

            return outerTable
                .GroupJoin(innerTable, outerKeySelector, innerKeySelector, (right, leftg) => new { leftg, right })
                .SelectMany(l => l.leftg.DefaultIfEmpty(), newrightrs);
        }

        public static IQueryable<TResult> FullOuterJoinDistinct<TOuterRow, TInnerRow, TKey, TResult>(
            this IQueryable<TOuterRow> outerTable,
            IQueryable<TInnerRow> innerTable,
            Expression<Func<TOuterRow, TKey>> outerKeySelector,
            Expression<Func<TInnerRow, TKey>> innerKeySelector,
            Expression<Func<TOuterRow, TInnerRow, TResult>> resultSelector)
            where TKey : IEquatable<TKey>
        {
            return outerTable
                .LeftOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector)
                .Union(outerTable
                    .RightOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector));
        }

        public static IQueryable<TResult> AntiJoin<TOuterRow, TAntiRow, TKey, TResult>(
            this IQueryable<TOuterRow> outerTable,
            IQueryable<TAntiRow> antiJoinedTable,
            Expression<Func<TOuterRow, TKey>> outerKeySelector,
            Expression<Func<TAntiRow, TKey>> antiJoinKeySelector,
            Expression<Func<TOuterRow, TAntiRow, TResult>> resultSelector)
            where TKey : IEquatable<TKey>
        {
            var sampleAnonLgR = new { leftg = default(IEnumerable<TOuterRow>), right = default(TAntiRow) };
            var parmLgR = Expression.Parameter(sampleAnonLgR.GetType(), "lgr");
            var argLeft = Expression.Constant(default(TOuterRow), typeof(TOuterRow));
            var argRight = Expression.PropertyOrField(parmLgR, "right");
            var newrightrs = CastSBody(Expression.Lambda(Expression.Invoke(resultSelector, argLeft, argRight), parmLgR), sampleAnonLgR, default(TResult));

            return antiJoinedTable
                .GroupJoin(outerTable, antiJoinKeySelector, outerKeySelector, (right, leftg) => new { leftg, right })
                .Where(lgr => !lgr.leftg.Any())
                .Select(newrightrs);
        }

        public static IQueryable<TResult> FullOuterJoin<TOuterTable, TInnerTable, TKey, TResult>(
            this IQueryable<TOuterTable> outerTable,
            IQueryable<TInnerTable> innerTable,
            Expression<Func<TOuterTable, TKey>> outerKeySelector,
            Expression<Func<TInnerTable, TKey>> innerKeySelector,
            Expression<Func<TOuterTable, TInnerTable, TResult>> resultSelector)
            where TKey : IEquatable<TKey>
        {
            return outerTable
                .LeftOuterJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector)
                .Concat(outerTable
                    .AntiJoin(innerTable, outerKeySelector, innerKeySelector, resultSelector));
        }

        private static Expression<Func<TP, TC, TResult>> CastSMBody<TP, TC, TResult>(LambdaExpression ex, TP unusedP, TC unusedC, TResult unusedRes) => (Expression<Func<TP, TC, TResult>>)ex;

        private static Expression<Func<TP, TResult>> CastSBody<TP, TResult>(LambdaExpression ex, TP unusedP, TResult unusedRes) => (Expression<Func<TP, TResult>>)ex;
    }
}
