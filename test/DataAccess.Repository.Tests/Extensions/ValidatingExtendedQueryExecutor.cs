// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatingExtendedQueryExecutor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The validating extended query executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Extensions
{
    using System;
    using System.Data.Linq;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Basic;

    using Extended;

    /// <summary>
    /// The validating extended query executor.
    /// </summary>
    public class ValidatingExtendedQueryExecutor : ExtendedQueryExecutor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingExtendedQueryExecutor"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="extensionsProvider">
        /// The extensions provider.
        /// </param>
        public ValidatingExtendedQueryExecutor(IRepository repository, IRepositoryExtensionsProvider extensionsProvider)
            : base(repository, extensionsProvider)
        {
        }

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
        protected override TResult ExecuteCore<TResult>(IQueryable query, QueryContext context, Expression expression)
        {
            var queryAsTable = query as ITable;
            if (queryAsTable != null)
            {
                var compiledQuery = queryAsTable.Context.Provider().Compile(expression);

                if (compiledQuery.SubQueries.Count > 0)
                {
                    throw new InvalidOperationException(String.Format(
                        CultureInfo.InvariantCulture,
                        "Expression:\n\r'{0}'\n\ris expanded to:\n\r'{1}'\n\rwith following SQL:\n\r'{2}'\n\rand {3} subqueries with SQL:\n\r{4}.\n\rPlease rewrite the query to avoid N+1 problem.",
                        context.Expression.ToString(),
                        expression.ToString(),
                        String.Join("\n\r", compiledQuery.QueryInfos.Select(qi => qi.CommandText).ToArray()),
                        compiledQuery.SubQueries.Count,                    
                        String.Join("\n\r", compiledQuery.SubQueries.Select(sq => sq.QueryInfo.CommandText).ToArray())));
                }
            }

            return base.ExecuteCore<TResult>(query, context, expression);
        }

        #endregion
    }
}