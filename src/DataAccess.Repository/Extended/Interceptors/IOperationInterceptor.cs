// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The i query interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors
{
    using Events;

    /// <summary>
    /// The operation (insert/update/delete) interceptor.
    /// </summary>
    public interface IOperationInterceptor
    {
        #region Public Methods
        /// <summary>
        /// Initializes the current interceptor.
        /// </summary>
        /// <param name="scope">The scope.</param>
        void Initialize(IScope scope);
        
        /// <summary>
        /// Rasied before Insert operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.</param>
        void OnInserting(OperationEventArgs eventArgs);

        /// <summary>
        /// Rasied before Update operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.</param>
        void OnUpdating(OperationEventArgs eventArgs);

        /// <summary>
        /// Rasied before Delete operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.</param>
        void OnDeleting(OperationEventArgs eventArgs);

        /// <summary>
        /// Rasied after Insert operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.</param>
        void OnInserted(OperationEventArgs eventArgs);

        /// <summary>
        /// Rasied after Update operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.</param>
        void OnUpdated(OperationEventArgs eventArgs);

        /// <summary>
        /// Rasied after Delete operation.
        /// </summary>
        /// <param name="eventArgs">The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.</param>
        void OnDeleted(OperationEventArgs eventArgs);
        #endregion
    }
}