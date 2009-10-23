// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The queryable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        #region Public Methods

        /// <summary>
        /// OrderBy extension method to apply multiple OrderBy/ThenBy's to query.
        /// </summary>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <param name="sortExpressions">
        /// The sort expressions.
        /// </param>
        /// <typeparam name="T">
        /// The entity type.
        /// </typeparam>
        /// <returns>
        /// Ordered query.
        /// </returns>
        public static IQueryable<T> OrderByMany<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] sortExpressions)
        {
            if (sortExpressions == null || sortExpressions.Length == 0)
            {
                return source;
            }

            var orderedSource = source.OrderBy(sortExpressions[0]);

            for (int i = 1; i < sortExpressions.Length; i++)
            {
                orderedSource = orderedSource.ThenBy(sortExpressions[i]);
            }

            return orderedSource;
        }

        /// <summary>
        /// The safe order by.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="sortExpression">
        /// The sort expression.
        /// </param>
        /// <typeparam name="T">
        /// Type of the query
        /// </typeparam>
        /// <returns>
        /// IQueryable with applied order expression
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Avoiding generic nesting seems not possible.")]
        public static IQueryable<T> SafeOrderBy<T>(this IQueryable<T> query, Expression<Func<T, object>> sortExpression)
        {
            if (sortExpression != null)
            {
                query = query.OrderBy(sortExpression);
            }

            return query;
        }

        /// <summary>
        /// The safe where.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filterExpression">
        /// The filter expression.
        /// </param>
        /// <typeparam name="T">
        /// Type of the query
        /// </typeparam>
        /// <returns>
        /// IQueryable with applied filter expression
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Avoiding generic nesting seems not possible.")]
        public static IQueryable<T> SafeWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>> filterExpression)
        {
            if (filterExpression != null)
            {
                query = query.Where<T>(filterExpression);
            }

            return query;
        }

        /// <summary>
        /// Non-typed Where method call.
        /// </summary>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New query with Where method call.
        /// </returns>
        public static IQueryable Where(this IQueryable source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return source.Provider.CreateQuery(source.Expression.Where(predicate));
        }

        #endregion
    }
}