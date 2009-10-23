// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedQueryExecutor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended query executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Basic;

    using Events;

    using Infrastructure.Linq;

    /// <summary>
    /// The extended query executor.
    /// </summary>
    public class ExtendedQueryExecutor : ContextExpressionVisitor<QueryContext>, IExtendedQueryExecutor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedQueryExecutor"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="extensionsProvider">
        /// The extensions provider.
        /// </param>
        public ExtendedQueryExecutor(IRepository repository, IRepositoryExtensionsProvider extensionsProvider)
        {
            this.Repository = repository;
            this.ExtensionsProvider = extensionsProvider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ExtensionsProvider.
        /// </summary>
        private IRepositoryExtensionsProvider ExtensionsProvider { get; set; }

        /// <summary>
        /// Gets or sets Repository.
        /// </summary>
        private IRepository Repository { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IExtendedQueryExecutor methods

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
        public TResult Execute<TResult>(QueryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.ExtensionsProvider.InitializeQueryContext(context);

            // first pass. processing all extension-specific method calls only 
            Expression firstPassOutputExpression = this.Visit(context.Expression, context);

            // QueryCreating stage
            var queryCreating = new QueryCreatingEventArgs(firstPassOutputExpression);

            this.ExtensionsProvider.OnQueryCreating(queryCreating, context);

            // LoadOptionsCreating stage
            var loadOptionsCreating = new LoadOptionsCreatingEventArgs(queryCreating.Expression, new LoadOptions());

            this.ExtensionsProvider.OnLoadOptionsCreating(loadOptionsCreating, context);

            // getting original query from repository
            var originalQuery = this.Repository.All(context.ElementType, loadOptionsCreating.LoadOptions);

            // QueryCreated stage
            var queryCreated = new QueryCreatedEventArgs(originalQuery);

            this.ExtensionsProvider.OnQueryCreated(queryCreated, context);

            // concatenating expression after load options creating event with expression after query created event
            Expression readyToExecuteExpression = new ExpressionConstantReplacer(context.RootQuery, queryCreated.Query.Expression).Visit(loadOptionsCreating.Expression);

            // PreExecute stage
            var preExecute = new PreExecuteEventArgs(readyToExecuteExpression);

            this.ExtensionsProvider.OnPreExecute(preExecute, context);

            return this.ExecuteCore<TResult>(originalQuery, context, preExecute.Expression);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The real execute implementation.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="query">
        /// The original queryable that will perform execute.
        /// </param>
        /// <param name="context">
        /// The query context context.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The result of the query.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TResult is the type of the result of the query.")]
        protected virtual TResult ExecuteCore<TResult>(IQueryable query, QueryContext context, Expression expression)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            return query.Provider.Execute<TResult>(expression);
        }

        /// <summary>
        /// Analyzes the method call expression provided as parameter and
        /// returns an appropiated member access.
        /// </summary>
        /// <param name="methodCall">
        /// The method call to analyze.
        /// </param>
        /// <param name="context">
        /// Current context.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCall, QueryContext context)
        {
            // note that we are not skipping any MethodCalls
            var e = new MethodCallVisitEventArgs(methodCall);

            // throwing away or replacing this method call if any interceptor found
            if (this.ExtensionsProvider.OnMethodCallVisit(e, context))
            {
                if (e.SubstituteExpression != e.MethodCall)
                {
                    return this.Visit(e.SubstituteExpression, context);
                }
            }

            return base.VisitMethodCall(methodCall, context);
        }

        #endregion
    }
}