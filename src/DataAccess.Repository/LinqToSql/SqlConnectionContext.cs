// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionContext.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sql connection context returned from SqlConnectionManager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System.Data.SqlClient;

    /// <summary>
    /// The sql connection context returned from SqlConnectionManager.
    /// </summary>
    public class SqlConnectionContext
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionContext"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="transaction">
        /// The transaction if available.
        /// </param>
        public SqlConnectionContext(SqlConnection connection, SqlTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        public SqlTransaction Transaction { get; set; }

        #endregion
    }
}