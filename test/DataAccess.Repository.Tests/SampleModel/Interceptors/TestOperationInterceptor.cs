// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOperationInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The test interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Interceptors
{
    using Extended;
    using Extended.Events;
    using Extended.Interceptors;

    /// <summary>
    /// The test interceptor.
    /// </summary>
    public class TestOperationInterceptor : OperationInterceptor<ITestScope>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the last deleted entity.
        /// </summary>
        /// <value>The last deleted entity.</value>
        public object LastDeletedEntity { get; set; }

        /// <summary>
        /// Gets or sets the last deleting entity.
        /// </summary>
        /// <value>The last deleting entity.</value>
        public object LastDeletingEntity { get; set; }

        /// <summary>
        /// Gets or sets the last inserted entity.
        /// </summary>
        /// <value>The last inserted entity.</value>
        public object LastInsertedEntity { get; set; }

        /// <summary>
        /// Gets or sets the last inserting entity.
        /// </summary>
        /// <value>The last inserting entity.</value>
        public object LastInsertingEntity { get; set; }

        /// <summary>
        /// Gets or sets the last updated entity.
        /// </summary>
        /// <value>The last updated entity.</value>
        public object LastUpdatedEntity { get; set; }

        /// <summary>
        /// Gets or sets the last updating entity.
        /// </summary>
        /// <value>The last updating entity.</value>
        public object LastUpdatingEntity { get; set; }

        /// <summary>
        /// Gets the public scope property.
        /// </summary>
        /// <value>The scope.</value>
        public IScope PublicScope
        {
            get
            {
                return this.Scope;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rasied after Delete operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public override void OnDeleted(OperationEventArgs eventArgs)
        {
            this.LastDeletedEntity = eventArgs.Entity;
        }

        /// <summary>
        /// Rasied before Delete operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnDeleting(OperationEventArgs eventArgs)
        {
            this.LastDeletingEntity = eventArgs.Entity;
        }

        /// <summary>
        /// Rasied after Insert operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public override void OnInserted(OperationEventArgs eventArgs)
        {
            this.LastInsertedEntity = eventArgs.Entity;
        }

        /// <summary>
        /// Rasied before Insert operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnInserting(OperationEventArgs eventArgs)
        {
            this.LastInsertingEntity = eventArgs.Entity;
        }

        /// <summary>
        /// Rasied after Update operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance contained the event data.
        /// </param>
        public override void OnUpdated(OperationEventArgs eventArgs)
        {
            this.LastUpdatedEntity = eventArgs.Entity;
        }

        /// <summary>
        /// Rasied before Update operation.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.OperationEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnUpdating(OperationEventArgs eventArgs)
        {
            this.LastUpdatingEntity = eventArgs.Entity;
        }

        #endregion
    }
}