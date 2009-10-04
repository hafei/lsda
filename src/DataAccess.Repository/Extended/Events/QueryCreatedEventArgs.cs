// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryCreatedEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   QueryCreated event arguments
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System.Linq;

    /// <summary>
    /// QueryCreated event arguments
    /// </summary>
    public class QueryCreatedEventArgs : InterceptorEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        public QueryCreatedEventArgs(IQueryable query)
        {
            this.Query = query;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public IQueryable Query { get; set; }

        #endregion
    }
}