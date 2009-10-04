// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadOptionsCreatingEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   QueryCreating event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System.Linq.Expressions;

    using Basic;

    /// <summary>
    /// LoadOptionsCreating event arguments.
    /// </summary>
    public class LoadOptionsCreatingEventArgs : InterceptorEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadOptionsCreatingEventArgs"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        public LoadOptionsCreatingEventArgs(Expression expression, LoadOptions loadOptions)
        {
            this.Expression = expression;
            this.LoadOptions = loadOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression { get; set; }

        /// <summary>
        /// Gets or sets the load options.
        /// </summary>
        /// <value>The load options.</value>
        public LoadOptions LoadOptions { get; set; }

        #endregion
    }
}