// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The projection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Extended.Attributes;

    using Infrastructure.Extensions;

    /// <summary>
    /// The projection extensions.
    /// </summary>
    public static class ProjectionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Selects typed projection with static config.
        /// </summary>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <returns>
        /// Null, because this method shouldn't be used directly, only in expressions.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "source", Justification = "This method is used as placeholder in expressions, so it's signature is significant.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TProjection is the type of the result.")]
        [InterceptVisit(typeof(ProjectionQueryInterceptor))]
        public static IEnumerable<TProjection> Select<TProjection>(this IEnumerable source)
            where TProjection : new()
        {
            return null;
        }

        /// <summary>
        /// Selects typed projection with instance config.
        /// </summary>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <param name="config">
        /// The instance config.
        /// </param>
        /// <returns>
        /// Null, because this method shouldn't be used directly, only in expressions.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "source", Justification = "This method is used as placeholder in expressions, so it's signature is significant.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "config", Justification = "This method is used as placeholder in expressions, so it's signature is significant.")]
        [InterceptVisit(typeof(ProjectionQueryInterceptor))]
        public static IEnumerable<TProjection> Select<TProjection>(this IEnumerable source, TProjection config)
            where TProjection : new()
        {
            return null;
        }

        /// <summary>
        /// Selects typed projection with static config.
        /// </summary>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <returns>
        /// New query with Select MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TProjection is the type of the result.")]
        [InterceptVisit(typeof(ProjectionQueryInterceptor))]
        public static IQueryable<TProjection> Select<TProjection>(this IQueryable source)
            where TProjection : new()
        {
            return MethodBase.GetCurrentMethod().AddToNewQuery<TProjection>(source);
        }

        /// <summary>
        /// Selects typed projection with instance config.
        /// </summary>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <param name="config">
        /// The instance config.
        /// </param>
        /// <returns>
        /// New query with Select MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TProjection is the type of the result.")]
        [InterceptVisit(typeof(ProjectionQueryInterceptor))]
        public static IQueryable<TProjection> Select<TProjection>(this IQueryable source, TProjection config)
            where TProjection : new()
        {
            return MethodBase.GetCurrentMethod().AddToNewQuery<TProjection>(source, Expression.Constant(config));
        }

        #endregion
    }
}