// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Extended.Attributes;

    /// <summary>
    /// The enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Selects typed projection.
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

        #endregion
    }
}