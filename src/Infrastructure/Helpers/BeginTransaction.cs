// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeginTransaction.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The TransactionScope helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Helpers
{
    using System.Transactions;

    /// <summary>
    /// The TransactionScope helper.
    /// </summary>
    public static class BeginTransaction
    {
        #region Public Methods

        /// <summary>
        /// Starts transaction with ReadCommitted isolation level.
        /// </summary>
        /// <returns>
        /// New TransactionScope.
        /// </returns>
        public static TransactionScope ReadCommitted()
        {
            var transactionOptions = new TransactionOptions();

            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        /// <summary>
        /// Starts transaction with ReadUncommitted isolation level.
        /// </summary>
        /// <returns>
        /// New TransactionScope.
        /// </returns>
        public static TransactionScope ReadUncommitted()
        {
            var transactionOptions = new TransactionOptions();

            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted;

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        /// <summary>
        /// Starts transaction with RepeatableRead isolation level.
        /// </summary>
        /// <returns>
        /// New TransactionScope.
        /// </returns>
        public static TransactionScope RepeatableRead()
        {
            var transactionOptions = new TransactionOptions();

            transactionOptions.IsolationLevel = IsolationLevel.RepeatableRead;

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        /// <summary>
        /// Starts transaction with Serializable isolation level.
        /// </summary>
        /// <returns>
        /// New TransactionScope.
        /// </returns>
        public static TransactionScope Serializable()
        {
            var transactionOptions = new TransactionOptions();

            transactionOptions.IsolationLevel = IsolationLevel.Serializable;

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        /// <summary>
        /// Starts transaction with Snapshot isolation level.
        /// </summary>
        /// <returns>
        /// New TransactionScope.
        /// </returns>
        public static TransactionScope Snapshot()
        {
            var transactionOptions = new TransactionOptions();

            transactionOptions.IsolationLevel = IsolationLevel.Snapshot;

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        #endregion
    }
}