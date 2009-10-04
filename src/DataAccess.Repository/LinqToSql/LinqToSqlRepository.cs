// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqToSqlRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Repository that uses Linq To SQL to store entity data
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Basic;

    using Mapping;

    /// <summary>
    /// Repository that uses Linq To SQL to store entity data
    /// </summary>
    public class LinqToSqlRepository : IRepository
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToSqlRepository"/> class.
        /// </summary>
        /// <param name="connectionManager">
        /// The sql connection manager.
        /// </param>
        /// <param name="mappingSourceManager">
        /// The mapping source manager.
        /// </param>
        public LinqToSqlRepository(ISqlConnectionManager connectionManager, IMappingSourceManager mappingSourceManager)
        {
            this.ConnectionManager = connectionManager;
            this.MappingSourceManager = mappingSourceManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sql connection manager.
        /// </summary>
        protected ISqlConnectionManager ConnectionManager { get; set; }

        /// <summary>
        /// Gets the mapping source manager.
        /// </summary>
        /// <value>The mapping source manager.</value>
        protected IMappingSourceManager MappingSourceManager { get; private set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IRepository methods

        /// <summary>
        /// Returns query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        public IQueryable<T> All<T>() where T : class
        {
            return this.All<T>(new LoadOptions());
        }

        /// <summary>
        /// Returns query for all entities of type T.
        /// Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        public IQueryable All(Type entityType)
        {
            return this.All(entityType, new LoadOptions());
        }

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        public virtual IQueryable<T> All<T>(LoadOptions loadOptions) where T : class
        {
            DataContext context = this.CreateReadOnlyDataContext(loadOptions);

            return context.GetTable<T>();
        }

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        /// Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        public virtual IQueryable All(Type entityType, LoadOptions loadOptions)
        {
            DataContext context = this.CreateReadOnlyDataContext(loadOptions);

            return context.GetTable(entityType);
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
        public virtual void Delete<T>(T entity) where T : class
        {
            using (DataContext context = this.CreateNoAssociationsDataContext())
            {
                Table<T> table = context.GetTable<T>();

                table.Attach(entity);

                table.DeleteOnSubmit(entity);

                context.SubmitChanges();
            }
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
        public virtual void Insert<T>(T entity) where T : class
        {
            using (DataContext context = this.CreateNoAssociationsDataContext())
            {
                Table<T> table = context.GetTable<T>();
                table.InsertOnSubmit(entity);

                context.SubmitChanges();
            }
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
        public virtual void Update<T>(T entity) where T : class
        {
            using (DataContext context = this.CreateNoAssociationsDataContext())
            {
                Table<T> table = context.GetTable<T>();
                table.Attach(entity, true);

                context.SubmitChanges();
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Converts LoadOptions to LinqToSql DataLoadOptions.
        /// </summary>
        /// <param name="dataContextLoadOptions">
        /// The data context load options.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        protected virtual void ApplyLoadOptions(DataLoadOptions dataContextLoadOptions, LoadOptions loadOptions)
        {
            foreach (LoadWithOption o in loadOptions.LoadWithOptions)
            {
                dataContextLoadOptions.LoadWith(o.Member);

                if (o.Association != null)
                {
                    dataContextLoadOptions.AssociateWith(o.Association);
                }
            }
        }

        /// <summary>
        /// Creates the data context with provided mapping source.
        /// </summary>
        /// <param name="mappingSource">
        /// The mapping source.
        /// </param>
        /// <returns>
        /// New DataContext.
        /// </returns>
        protected virtual DataContext CreateDataContext(MappingSource mappingSource)
        {
            var sqlContext = this.ConnectionManager.GetConnection();

            var dataContext = new DataContext(sqlContext.Connection, mappingSource);

            if (sqlContext.Transaction != null)
            {
                dataContext.Transaction = sqlContext.Transaction;
            }

            return dataContext;
        }

        /// <summary>
        /// Creates the data context without association support.
        /// </summary>
        /// <returns>
        /// The data context without association support
        /// </returns>
        protected DataContext CreateNoAssociationsDataContext()
        {
            return this.CreateDataContext(this.MappingSourceManager.NoAssociationsMappingSource);
        }

        /// <summary>
        /// Creates the read only data context.
        /// </summary>
        /// <returns>
        /// The read only data context.
        /// </returns>
        protected DataContext CreateReadOnlyDataContext()
        {
            var context = this.CreateDataContext(this.MappingSourceManager.MappingSource);
            context.ObjectTrackingEnabled = false;
            context.DeferredLoadingEnabled = false;

            return context;
        }

        /// <summary>
        /// Creates the read only data context with LoadOptions.
        /// </summary>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// The read only data context with LoadOptions.
        /// </returns>
        protected DataContext CreateReadOnlyDataContext(LoadOptions loadOptions)
        {
            DataContext context = this.CreateReadOnlyDataContext();

            if (loadOptions != null && !loadOptions.IsEmpty)
            {
                context.LoadOptions = this.GetDataContextLoadOptions(loadOptions);
            }

            return context;
        }

        /// <summary>
        /// Creating LinqToSql DataLoadOptions from provided LoadOptions.
        /// </summary>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// LinqToSql DataLoadOptions.
        /// </returns>
        protected DataLoadOptions GetDataContextLoadOptions(LoadOptions loadOptions)
        {
            var dataContextLoadOptions = new DataLoadOptions();

            this.ApplyLoadOptions(dataContextLoadOptions, loadOptions);

            return dataContextLoadOptions;
        }

        #endregion
    }
}