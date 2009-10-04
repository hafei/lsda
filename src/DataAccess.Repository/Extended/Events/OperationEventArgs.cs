// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   OperationInterceptor Method event arguments
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Events
{
    using System;

    /// <summary>
    /// Operation Method event arguments
    /// </summary>
    public class OperationEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public OperationEventArgs(object entity)
        {
            this.Entity = entity;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public object Entity { get; private set; }

        #endregion
    }
}