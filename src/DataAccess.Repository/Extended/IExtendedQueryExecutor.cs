// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendedQueryExecutor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended query executor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The extended query executor interface.
    /// </summary>
    public interface IExtendedQueryExecutor
    {
        #region Public Methods

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="context">
        /// The query context.
        /// </param>
        /// <returns>
        /// Result of the query.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TResult is the type of the result of the query.")]
        TResult Execute<TResult>(QueryContext context);

        #endregion
    }
}