// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreExecuteEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   PreExecute event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System.Linq.Expressions;

    /// <summary>
    /// PreExecute event arguments.
    /// </summary>
    public class PreExecuteEventArgs : InterceptorEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreExecuteEventArgs"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public PreExecuteEventArgs(Expression expression)
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