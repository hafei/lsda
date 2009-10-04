// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlConnectionManager.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sql connection manager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The sql connection manager interface.
    /// </summary>
    public interface ISqlConnectionManager
    {
        #region Public Methods

        /// <summary>
        /// Gets the sql connection and transaction if present.
        /// </summary>
        /// <returns>
        /// SqlConnectionContext with connection and transaction if available.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method chosen by design.")]
        SqlConnectionContext GetConnection();

        #endregion
    }
}