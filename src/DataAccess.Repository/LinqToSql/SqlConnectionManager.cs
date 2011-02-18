// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionManager.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sql connection manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Transactions;

    using IsolationLevel = System.Transactions.IsolationLevel;
    using SqlIsolationLevel = System.Data.IsolationLevel;

    /// <summary>
    /// The sql connection manager.
    /// </summary>
    public class SqlConnectionManager : ISqlConnectionManager, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The transaction enlistments map.
        /// </summary>
        private static readonly Dictionary<string, SqlConnectionEnlistment> TransactionEnlistmentsMap = new Dictionary<string, SqlConnectionEnlistment>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionManager"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public SqlConnectionManager(IConnectionString connectionString)
        {
            this.ConnectionString = connectionString;

            this.CreatedConnections = new List<SqlConnection>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ConnectionString.
        /// </summary>
        public IConnectionString ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets CreatedConnections.
        /// </summary>
        private List<SqlConnection> CreatedConnections { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IDisposable methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region ISqlConnectionManager methods

        /// <summary>
        /// Gets the sql connection and transaction if present.
        /// </summary>
        /// <returns>
        /// SqlConnectionContext with connection and transaction if available.
        /// </returns>
        public SqlConnectionContext GetConnection()
        {
            var connectionEnlistment = this.TryEnlistConnection();

            if (connectionEnlistment != null)
            {
                return new SqlConnectionContext(connectionEnlistment.SqlConnection, connectionEnlistment.SqlTransaction);
            }

            return new SqlConnectionContext(this.CreateConnection(), null);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var connection in this.CreatedConnections)
                {
                    if (connection != null && connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }

                this.CreatedConnections.Clear();
            }
        }

        /// <summary>
        /// Cleanups the enlistment.
        /// </summary>
        /// <param name="transactionLocalIdentifier">
        /// The transaction local identifier.
        /// </param>
        private static void CleanupEnlistment(string transactionLocalIdentifier)
        {
            lock (TransactionEnlistmentsMap)
            {
                TransactionEnlistmentsMap.Remove(transactionLocalIdentifier);
            }
        }

        /// <summary>
        /// Gets sql isolation level from System.Transactions.IsolationLevel.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The sql isolation level.
        /// </returns>
        private static SqlIsolationLevel GetSqlIsolationLevel(Transaction transaction)
        {
            var isolationLevel = transaction.IsolationLevel;

            switch (isolationLevel)
            {
                case IsolationLevel.Serializable:
                    return SqlIsolationLevel.Serializable;

                case IsolationLevel.RepeatableRead:
                    return SqlIsolationLevel.RepeatableRead;

                case IsolationLevel.ReadCommitted:
                    return SqlIsolationLevel.ReadCommitted;

                case IsolationLevel.ReadUncommitted:
                    return SqlIsolationLevel.ReadUncommitted;

                case IsolationLevel.Snapshot:
                    return SqlIsolationLevel.Snapshot;
            }

            throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Unknown isolation level: {0}.", isolationLevel));
        }

        /// <summary>
        /// Creates the sql connection.
        /// </summary>
        /// <returns>
        /// New sql connection.
        /// </returns>
        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(this.ConnectionString.ConnectionString);

            lock (this.CreatedConnections)
            {
                this.CreatedConnections.Add(connection);
            }

            return connection;
        }

        /// <summary>
        /// Tries to create and enlist the connection in current transaction.
        /// </summary>
        /// <returns>
        /// SqlConnectionEnlistment if current transaction exists.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "On cleanup.")]
        private SqlConnectionEnlistment TryEnlistConnection()
        {
            if (Transaction.Current == null)
            {
                return null;
            }

            string transactionLocalIdentifier = Transaction.Current.TransactionInformation.LocalIdentifier;

            SqlConnectionEnlistment connectionEnlistment;

            // todo: optimize lock if possible
            lock (TransactionEnlistmentsMap)
            {
                if (!TransactionEnlistmentsMap.TryGetValue(transactionLocalIdentifier, out connectionEnlistment))
                {
                    SqlConnection connection = new SqlConnection(this.ConnectionString.ConnectionString);
                    SqlTransaction transaction;
                    try
                    {
                        connection.Open();

                        transaction = connection.BeginTransaction(GetSqlIsolationLevel(Transaction.Current));
                    }
                    catch
                    {
                        connection.Close();

                        throw;
                    }

                    connectionEnlistment = new SqlConnectionEnlistment(transactionLocalIdentifier, connection, transaction);

                    TransactionEnlistmentsMap.Add(transactionLocalIdentifier, connectionEnlistment);

                    Transaction.Current.EnlistVolatile(connectionEnlistment, EnlistmentOptions.None);
                }
            }

            return connectionEnlistment;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// The sql connection enlistment.
        /// </summary>
        private class SqlConnectionEnlistment : IEnlistmentNotification
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlConnectionEnlistment"/> class.
            /// </summary>
            /// <param name="transactionLocalIdentifier">
            /// The transaction Local Identifier.
            /// </param>
            /// <param name="connection">
            /// The sql connection.
            /// </param>
            /// <param name="sqlTtransaction">
            /// The sql transaction.
            /// </param>
            public SqlConnectionEnlistment(string transactionLocalIdentifier, SqlConnection connection, SqlTransaction sqlTtransaction)
            {
                this.TransactionLocalIdentifier = transactionLocalIdentifier;
                this.SqlConnection = connection;
                this.SqlTransaction = sqlTtransaction;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the sql connection.
            /// </summary>
            public SqlConnection SqlConnection { get; private set; }

            /// <summary>
            /// Gets the sql transaction.
            /// </summary>
            public SqlTransaction SqlTransaction { get; private set; }

            /// <summary>
            /// Gets the transaction local identifier.
            /// </summary>
            public string TransactionLocalIdentifier { get; private set; }

            #endregion

            #region Implemented Interfaces (Methods)

            #region IEnlistmentNotification methods

            /// <summary>
            /// Notifies an enlisted object that a transaction is being committed.
            /// </summary>
            /// <param name="enlistment">
            /// An <see cref="T:System.Transactions.Enlistment"/> object used to send a response to the transaction manager.
            /// </param>
            public void Commit(Enlistment enlistment)
            {
                if (enlistment == null)
                {
                    throw new ArgumentNullException("enlistment");
                }

                try
                {
                    this.SqlTransaction.Commit();
                }
                finally
                {
                    this.SqlConnection.Close();
                }

                CleanupEnlistment(this.TransactionLocalIdentifier);

                enlistment.Done();
            }

            /// <summary>
            /// Notifies an enlisted object that the status of a transaction is in doubt.
            /// </summary>
            /// <param name="enlistment">
            /// An <see cref="T:System.Transactions.Enlistment"/> object used to send a response to the transaction manager.
            /// </param>
            public void InDoubt(Enlistment enlistment)
            {
                if (enlistment == null)
                {
                    throw new ArgumentNullException("enlistment");
                }

                enlistment.Done();
            }

            /// <summary>
            /// Notifies an enlisted object that a transaction is being prepared for commitment.
            /// </summary>
            /// <param name="preparingEnlistment">
            /// A <see cref="T:System.Transactions.PreparingEnlistment"/> object used to send a response to the transaction manager.
            /// </param>
            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                if (preparingEnlistment == null)
                {
                    throw new ArgumentNullException("preparingEnlistment");
                }

                preparingEnlistment.Prepared();
            }

            /// <summary>
            /// Notifies an enlisted object that a transaction is being rolled back (aborted).
            /// </summary>
            /// <param name="enlistment">
            /// A <see cref="T:System.Transactions.Enlistment"/> object used to send a response to the transaction manager.
            /// </param>
            public void Rollback(Enlistment enlistment)
            {
                if (enlistment == null)
                {
                    throw new ArgumentNullException("enlistment");
                }

                try
                {
                    this.SqlTransaction.Rollback();
                }
                finally
                {
                    this.SqlConnection.Close();
                }

                CleanupEnlistment(this.TransactionLocalIdentifier);

                enlistment.Done();
            }

            #endregion

            #endregion
        }

        #endregion
    }
}