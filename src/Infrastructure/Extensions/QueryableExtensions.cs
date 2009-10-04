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
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        #region Public Methods

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
            return source.Provider.CreateQuery(source.Expression.Where(predicate));
        }

        #endregion
    }
}