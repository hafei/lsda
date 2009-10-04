// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryQueryable.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Memory Repository Queryable object
    /// </summary>
    /// <typeparam name="T">
    /// Entity type
    /// </typeparam>
    internal class MemoryQueryable<T> : IQueryable<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryQueryable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public MemoryQueryable(IQueryProvider provider)
        {
            this.Provider = provider;
            this.Expression = Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryQueryable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="expression">The expression.</param>
        public MemoryQueryable(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }
        #endregion

        #region Implemented Interfaces (Properties)

        #region IQueryable properties

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider { get; private set; }

        #endregion

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
        public IEnumerator<T> GetEnumerator()
        {
            return ((IQueryable)this).Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        #endregion

        #endregion
    }
}