// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionString.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    /// <summary>
    /// Connection String interface. 
    /// </summary>
    public interface IConnectionString
    {
        #region Properties

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; }

        #endregion
    }
}