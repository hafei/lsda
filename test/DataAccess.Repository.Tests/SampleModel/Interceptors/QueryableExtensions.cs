// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The queryable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Interceptors
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Extended.Attributes;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Test extension method.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="source">
        /// The source query.
        /// </param>
        /// <returns>
        /// New query with method call added.
        /// </returns>
        [InterceptVisit(typeof(TestInterceptor))]
        public static IQueryable<T> TestMethod<T>(this IQueryable<T> source)
        {
            return source.Provider.CreateQuery<T>(Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)), source.Expression));
        }

        #endregion
    }
}