// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The i query interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors
{
    using Events;

    /// <summary>
    /// The i query interceptor.
    /// </summary>
    public interface IQueryInterceptor
    {
        #region Public Methods

        /// <summary>
        /// Initializes the current interceptor.
        /// </summary>
        /// <param name="queryContext">
        /// The context of the query.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        void Initialize(QueryContext queryContext, IScope scope);

        /// <summary>
        /// The MethodCallVisit stage handler.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        void OnMethodCallVisit(MethodCallVisitEventArgs e);

        /// <summary>
        /// The PreExecute stage handler.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        void OnPreExecute(PreExecuteEventArgs e);

        /// <summary>
        /// The QueryCreating stage handler.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        void OnQueryCreating(QueryCreatingEventArgs e);

        /// <summary>
        /// The QueryCreated stage handler.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        void OnQueryCreated(QueryCreatedEventArgs e);

        /// <summary>
        /// The LoadOptionsCreating stage handler.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        void OnLoadOptionsCreating(LoadOptionsCreatingEventArgs e);

        #endregion
    }
}