// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableLinqToSqlRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The disposable linq to sql repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    using Mapping;

    /// <summary>
    /// The disposable linq to sql repository.
    /// </summary>
    public class DisposableLinqToSqlRepository : LinqToSqlRepository, IDisposable
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLinqToSqlRepository"/> class.
        /// </summary>
        /// <param name="connectionManager">
        /// The sql connection manager.
        /// </param>
        /// <param name="mappingSourceManager">
        /// The mapping source manager.
        /// </param>
        public DisposableLinqToSqlRepository(ISqlConnectionManager connectionManager, IMappingSourceManager mappingSourceManager)
            : base(connectionManager, mappingSourceManager)
        {
            this.CreatedDataContexts = new List<DataContext>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets CreatedDataContexts.
        /// </summary>
        private List<DataContext> CreatedDataContexts { get; set; }

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

        #endregion

        #region Methods

        /// <summary>
        /// Creates the data context with provided mapping source.
        /// </summary>
        /// <param name="mappingSource">
        /// The mapping source.
        /// </param>
        /// <returns>
        /// New DataContext.
        /// </returns>
        protected override DataContext CreateDataContext(MappingSource mappingSource)
        {
            var context = base.CreateDataContext(mappingSource);

            this.CreatedDataContexts.Add(context);

            return context;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var dataContext in this.CreatedDataContexts)
                {
                    if (dataContext != null)
                    {
                        dataContext.Dispose();
                    }
                }

                this.CreatedDataContexts.Clear();
            }
        }

        #endregion
    }
}