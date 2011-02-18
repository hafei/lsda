// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedQuery.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Infrastructure.Helpers;

    /// <summary>
    /// The extended query.
    /// </summary>
    /// <typeparam name="T">
    /// Element type.
    /// </typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ExtendedQuery<T> have nothing to do with collection.")]
    public sealed class ExtendedQuery<T> : IOrderedQueryable<T>, IQueryProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedQuery{T}"/> class.
        /// </summary>
        /// <param name="queryExecutor">
        /// The query executor.
        /// </param>
        public ExtendedQuery(IExtendedQueryExecutor queryExecutor)
        {
            this.QueryExecutor = queryExecutor;
            this.RootQuery = this;
            this.Expression = Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedQuery{T}"/> class.
        /// </summary>
        /// <param name="queryExecutor">
        /// The query executor.
        /// </param>
        /// <param name="rootQuery">
        /// The root query that came from extended repository.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public ExtendedQuery(IExtendedQueryExecutor queryExecutor, IQueryable rootQuery, Expression expression)
        {
            this.QueryExecutor = queryExecutor;
            this.RootQuery = rootQuery;
            this.Expression = expression;
        }

        #endregion

        #region Implemented Interfaces (Properties)

        #region IQueryable properties

        /// <summary>
        ///   Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref = "T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///   A <see cref = "T:System.Type" /> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        Type IQueryable.ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        ///   Gets the expression tree that is associated with the instance of <see cref = "T:System.Linq.IQueryable" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///   The <see cref = "T:System.Linq.Expressions.Expression" /> that is associated with this instance of <see cref = "T:System.Linq.IQueryable" />.
        /// </returns>
        Expression IQueryable.Expression
        {
            get
            {
                return this.Expression;
            }
        }

        /// <summary>
        ///   Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///   The <see cref = "T:System.Linq.IQueryProvider" /> that is associated with this data source.
        /// </returns>
        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this;
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the expression of the current query.
        /// </summary>
        private Expression Expression { get; set; }

        /// <summary>
        ///   Gets or sets the QueryExecutor.
        /// </summary>
        private IExtendedQueryExecutor QueryExecutor { get; set; }

        /// <summary>
        ///   Gets or sets the root query that came from extended repository.
        /// </summary>
        private IQueryable RootQuery { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IQueryable)this).Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "ExtendedQuery(" + typeof(T).Name + ")";
        }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IEnumerable methods

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IQueryProvider methods

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Type elementType = TypeSystem.GetElementType(expression.Type);
            if (!typeof(IQueryable<>).MakeGenericType(elementType).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("Invalid type expression", "expression");
            }

            return (IQueryable)Activator.CreateInstance(
                typeof(ExtendedQuery<>).MakeGenericType(elementType), 
                new object[] { this.QueryExecutor, this.RootQuery, expression });
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the element.
        /// </typeparam>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("Invalid type expression", "expression");
            }

            return new ExtendedQuery<TElement>(this.QueryExecutor, this.RootQuery, expression);
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        object IQueryProvider.Execute(Expression expression)
        {
            return ((IQueryProvider)this).Execute<object>(expression);
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            var queryContext = new QueryContext(this.RootQuery, expression);

            return this.QueryExecutor.Execute<TResult>(queryContext);
        }

        #endregion

        #endregion
    }
}