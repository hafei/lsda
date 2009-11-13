// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Repository proxy that supports methods and types interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Basic;

    using Events;

    using Infrastructure.Extensions;

    using Interceptors;

    /// <summary>
    /// Repository proxy that supports methods and types interception.
    /// </summary>
    public class ExtendedRepository : IExtendedRepository
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedRepository"/> class.
        /// </summary>
        /// <param name="innerRepository">
        /// The inner repository.
        /// </param>
        /// <param name="queryExecutor">
        /// The query executor.
        /// </param>
        /// <param name="repositoryExtensionsProvider">
        /// The repository extensions provider.
        /// </param>
        public ExtendedRepository(IRepository innerRepository, IExtendedQueryExecutor queryExecutor, IRepositoryExtensionsProvider repositoryExtensionsProvider)
        {
            this.InnerRepository = innerRepository;
            this.QueryExecutor = queryExecutor;
            this.RepositoryExtensionsProvider = repositoryExtensionsProvider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the inner repository.
        /// </summary>
        /// <value>The inner repository.</value>
        private IRepository InnerRepository { get; set; }

        /// <summary>
        /// Gets or sets the query executor.
        /// </summary>
        /// <value>The query executor.</value>
        private IExtendedQueryExecutor QueryExecutor { get; set; }

        /// <summary>
        /// Gets or sets the repository extensions provider.
        /// </summary>
        /// <value>The repository extensions provider.</value>
        private IRepositoryExtensionsProvider RepositoryExtensionsProvider { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IExtendedRepository methods

        /// <summary>
        /// Returns the query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <returns>
        /// The query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        public IQueryable<T> All<T>() where T : class
        {
            return new ExtendedQuery<T>(this.QueryExecutor);
        }

        /// <summary>
        /// Returns the query for all entities of specified type.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <returns>
        /// The query for all entities of specified type.
        /// </returns>
        public IQueryable All(Type entityType)
        {
            return (IQueryable) Activator.CreateInstance(
                                    typeof(ExtendedQuery<>).MakeGenericType(entityType), 
                                    new object[] { this.QueryExecutor });
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Delete<T>(T entity) where T : class
        {
            this.ExecuteInterceptableOperation(
                entity, 
                x => x.OnDeleting, 
                e => this.InnerRepository.Delete(e), 
                x => x.OnDeleted);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Insert<T>(T entity) where T : class
        {
            this.ExecuteInterceptableOperation(
                entity, 
                x => x.OnInserting, 
                e => this.InnerRepository.Insert(e), 
                x => x.OnInserted);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Update<T>(T entity) where T : class
        {
            this.ExecuteInterceptableOperation(
                entity, 
                x => x.OnUpdating, 
                e => this.InnerRepository.Update(e), 
                x => x.OnUpdated);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Executes the interceptable operation.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="preOperation">
        /// The pre operation.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="postOperation">
        /// The post operation.
        /// </param>
        private void ExecuteInterceptableOperation<T>(
            T entity, 
            Func<IOperationInterceptor, Action<OperationEventArgs>> preOperation, 
            Action<T> operation, 
            Func<IOperationInterceptor, Action<OperationEventArgs>> postOperation)
        {
            var interceptorEventArguments = new OperationEventArgs(entity);

            var operationInterceptors = this.RepositoryExtensionsProvider.InitializeOperationInterceptors<T>();

            operationInterceptors.Select(preOperation).Multicast()(interceptorEventArguments);

            operation(entity);

            operationInterceptors.Select(postOperation).Multicast()(interceptorEventArguments);
        }

        #endregion
    }
}