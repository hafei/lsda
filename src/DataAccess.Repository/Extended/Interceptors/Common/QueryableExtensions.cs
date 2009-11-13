// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The queryable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Extended.Attributes;

    using Infrastructure.Extensions;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Expands expression properties in the query.
        /// </summary>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <typeparam name="T">
        /// The type of element of the query.
        /// </typeparam>
        /// <returns>
        /// Source IQueryable with Expand MethodCall added.
        /// </returns>
        [InterceptVisit(typeof(ExpressionExpanderQueryInterceptor))]
        public static IQueryable<T> Expand<T>(this IQueryable<T> source)
        {
            return MethodBase.GetCurrentMethod().AddToQuery(source);
        }

        /// <summary>
        /// Loads sprcified entities along with root one.
        /// </summary>
        /// <typeparam name="T">
        /// The type of element of the query.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// Source IQueryable with LoadWith MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        [InterceptVisit(typeof(LoadWithQueryInterceptor))]
        public static IQueryable<T> LoadWith<T>(this IQueryable<T> source, Expression<Func<T, object>> predicate)
        {
            return MethodBase.GetCurrentMethod().AddToQuery(source, Expression.Quote(predicate));
        }

        /// <summary>
        /// Loads the with.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the query element.
        /// </typeparam>
        /// <typeparam name="TParent">
        /// The type of the inner element.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// Source IQueryable with LoadWith MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        [InterceptVisit(typeof(LoadWithQueryInterceptor))]
        public static IQueryable<T> LoadWith<T, TParent>(this IQueryable<T> source, Expression<Func<TParent, object>> predicate)
        {
            return MethodBase.GetCurrentMethod().AddToQuery<T, TParent>(source, Expression.Quote(predicate));
        }

        /// <summary>
        /// Loads the with.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the query element.
        /// </typeparam>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="projection">
        /// The projection instance to infer projection type from.
        /// </param>
        /// <returns>
        /// Source IQueryable with Select MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        [InterceptVisit(typeof(ProjectionQueryInterceptor))]
        public static IQueryable<TProjection> Select<T, TProjection>(this IQueryable<T> source, TProjection projection)
        {
            return MethodBase.GetCurrentMethod().AddToNewQuery<T, TProjection>(source, Expression.Constant(projection));
        }

        #endregion
    }
}