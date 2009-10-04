// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodCallVisitEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The method call visit event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System.Linq.Expressions;

    /// <summary>
    /// The method call visit event args.
    /// </summary>
    public class MethodCallVisitEventArgs : InterceptorEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallVisitEventArgs"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public MethodCallVisitEventArgs(MethodCallExpression expression)
        {
            // note that substitute expression now defaults to original expression
            this.SubstituteExpression = this.MethodCall = expression;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the original expression.
        /// </summary>
        /// <value>The original expression.</value>
        public MethodCallExpression MethodCall { get; private set; }

        /// <summary>
        /// Gets or sets the expression to place into thee.
        /// </summary>
        /// <remarks>
        /// Defaults to original expression.
        /// </remarks>
        public Expression SubstituteExpression { get; set; }

        #endregion
    }
}