// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Base abstract class for interceptors that does nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors
{
    using System;
    using System.Globalization;

    using Events;

    using Infrastructure.Linq;

    /// <summary>
    /// Base abstract class for interceptors that does nothing.
    /// </summary>
    /// <typeparam name="TScope">
    /// Type of the scope.
    /// </typeparam>
    public abstract class QueryInterceptor<TScope> : ExpressionVisitor, IQueryInterceptor
        where TScope : IScope
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context of the query.
        /// </summary>
        /// <value>The context of the query.</value>
        protected QueryContext QueryContext { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        protected TScope Scope { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IQueryInterceptor methods

        /// <summary>
        /// Initializes the current interceptor.
        /// </summary>
        /// <param name="queryContext">
        /// The context of the query.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <remarks>
        /// Remember to call base implementation if overriden.
        /// </remarks>
        public virtual void Initialize(QueryContext queryContext, IScope scope)
        {
            if (!(scope is TScope))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Argument scope is expected to be of type {0}, but was of type {1}.", typeof(TScope).Name, scope.GetType().Name), "scope");
            }

            this.QueryContext = queryContext;
            this.Scope = (TScope) scope;
        }

        /// <summary>
        /// The MethodCallVisit stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.MethodCallVisitEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// Base implementation is empty.
        /// </remarks>
        public virtual void OnMethodCallVisit(MethodCallVisitEventArgs e)
        {
        }

        /// <summary>
        /// The PreExecute stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.PreExecuteEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// Base implementation is empty.
        /// </remarks>
        public virtual void OnPreExecute(PreExecuteEventArgs e)
        {
        }

        /// <summary>
        /// The QueryCreating stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// Base implementation is empty.
        /// </remarks>
        public virtual void OnQueryCreating(QueryCreatingEventArgs e)
        {
        }

        /// <summary>
        /// The QueryCreated stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.QueryCreatedEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// Base implementation is empty.
        /// </remarks>
        public virtual void OnQueryCreated(QueryCreatedEventArgs e)
        {
        }

        /// <summary>
        /// The LoadOptionsCreating stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.LoadOptionsCreatingEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// Base implementation is empty.
        /// </remarks>
        public virtual void OnLoadOptionsCreating(LoadOptionsCreatingEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}