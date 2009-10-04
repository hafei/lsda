// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryCreatingEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The PreQueryCreating event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System.Linq.Expressions;

    /// <summary>
    /// The PreQueryCreating event arguments.
    /// </summary>
    public class QueryCreatingEventArgs : InterceptorEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCreatingEventArgs"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public QueryCreatingEventArgs(Expression expression)
        {
            this.Expression = expression;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression { get; set; }

        #endregion
    }
}