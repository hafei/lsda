// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Context for query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Interceptors;

    /// <summary>
    /// Context for query.
    /// </summary>
    public class QueryContext
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class.
        /// </summary>
        /// <param name="rootQuery">
        /// The root query that came from extended repository.
        /// </param>
        /// <param name="expression">
        /// The original expression of the query.
        /// </param>
        public QueryContext(IQueryable rootQuery, Expression expression)
        {
            this.RootQuery = rootQuery;
            this.ElementType = rootQuery.ElementType;
            this.Expression = expression;

            this.Interceptors = new Dictionary<Type, IQueryInterceptor>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element type of the query.
        /// </summary>
        public Type ElementType { get; private set; }

        /// <summary>
        /// Gets the original expression of the query.
        /// </summary>
        /// <value>The original expression of the query.</value>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <value>The interceptors.</value>
        public Dictionary<Type, IQueryInterceptor> Interceptors { get; private set; }

        /// <summary>
        /// Gets the root query that came from extended repository.
        /// </summary>
        /// <value>The extended root query.</value>
        public IQueryable RootQuery { get; private set; }

        #endregion
    }
}