// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Base abstract class for interceptors that does nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors
{
    using System;
    using System.Globalization;

    using LogicSoftware.DataAccess.Repository.Extended.Events;

    /// <summary>
    /// Base abstract class for interceptors that does nothing.
    /// </summary>
    /// <typeparam name="TScope">
    /// Type of the scope.
    /// </typeparam>
    public abstract class OperationInterceptor<TScope> : IOperationInterceptor
        where TScope : IScope
    {
        #region Properties

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        protected TScope Scope { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IOperationInterceptor methods

        /// <summary>
        /// Initializes the current interceptor.
        /// </summary>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <remarks>
        /// Remember to call base implementation if overriden.
        /// </remarks>
        public virtual void Initialize(IScope scope)
        {
            if (!(scope is TScope))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Argument scope is expected to be of type {0}, but was of type {1}.", typeof(TScope).Name, scope.GetType().Name), "scope");
            }

            this.Scope = (TScope) scope;
        }

        /// <summary>
        /// Rasied after Delete operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public virtual void OnDeleted(OperationEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Rasied before Delete operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void OnDeleting(OperationEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Rasied after Insert operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public virtual void OnInserted(OperationEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Rasied before Insert operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void OnInserting(OperationEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Rasied after Update operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public virtual void OnUpdated(OperationEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Rasied before Update operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void OnUpdating(OperationEventArgs eventArgs)
        {
        }

        #endregion

        #endregion
    }
}