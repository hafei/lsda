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

    using Attributes;

    using Infrastructure.Extensions;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        #region Public Methods

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
        /// Source IQueryable with LoadWith MethodCall added;
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
        /// Source IQueryable with LoadWith MethodCall added;
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
        /// <typeparam name="TView">
        /// The type of the view element.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// Source IQueryable with Select MethodCall added;
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Seems impossible.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        [InterceptVisit(typeof(ViewQueryInterceptor))]
        public static IQueryable<TView> Select<T, TView>(this IQueryable<T> source)
        {
            return MethodBase.GetCurrentMethod().AddToNewQuery<T, TView>(source);
        }

        #endregion
    }
}