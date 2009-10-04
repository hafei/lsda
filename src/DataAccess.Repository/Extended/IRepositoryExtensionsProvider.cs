// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryExtensionsProvider.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Handles events and query context for data interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Events;
    using LogicSoftware.DataAccess.Repository.Extended.Interceptors;

    /// <summary>
    /// Handles events and query context for data interception.
    /// </summary>
    public interface IRepositoryExtensionsProvider
    {
        #region Public Methods

        /// <summary>
        /// Initializes the query context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void InitializeQueryContext(QueryContext context);

        /// <summary>
        /// Notifies interceptors about MethodCallVisit stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.MethodCallVisitEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if method call was interepted, <c>false</c> otherwise.
        /// </returns>
        bool OnMethodCallVisit(MethodCallVisitEventArgs e, QueryContext context);

        /// <summary>
        /// Notifies interceptors about PreExecute stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.PreExecuteEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        bool OnPreExecute(PreExecuteEventArgs e, QueryContext context);

        /// <summary>
        /// Notifies interceptor about PreQueryCreating stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        bool OnQueryCreating(QueryCreatingEventArgs e, QueryContext context);

        /// <summary>
        /// Notifies interceptors about QueryCreated stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatedEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        bool OnQueryCreated(QueryCreatedEventArgs e, QueryContext context);

        /// <summary>
        /// Notifies interceptor about LoadOptionsCreating stage in query execution.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.LoadOptionsCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c>, if at least one interceptor has been executed, <c>false</c> otherwise.
        /// </returns>
        bool OnLoadOptionsCreating(LoadOptionsCreatingEventArgs e, QueryContext context);

        /// <summary>
        /// Initializes the operation interceptor.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>The operation interceptor.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "by design, to avoid typeof(T) at caller")]
        IEnumerable<IOperationInterceptor> InitializeOperationInterceptors<T>();
        #endregion
    }
}