// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryIndexesCache.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using Basic;

    /// <summary>
    /// Indexes Cache for Memory Repository explicit joins
    /// </summary>
    internal class QueryIndexesCache
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryIndexesCache"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public QueryIndexesCache(IRepository repository)
        {
            this.Repository = repository;
            this.TableIndexesCaches = new Dictionary<Type, object>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>The repository.</value>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        private IRepository Repository { get; set; }

        /// <summary>
        /// Gets or sets the table indexes caches.
        /// </summary>
        /// <value>The table indexes caches.</value>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        private Dictionary<Type, object> TableIndexesCaches { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns all entities with specified property value.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// All entities with specified property value.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        internal List<T> GetAllByPropertyValue<T>(PropertyInfo property, object value) where T : class
        {
            TableIndexes<T> tableCache = GetTableIndexesCache<T>();

            return tableCache[property][value];
        }

        /// <summary>
        /// Gets the single entity with specified property value.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The single entity with specified property value.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        internal T GetSingleByPropertyValue<T>(PropertyInfo property, object value) where T : class
        {
            return this.GetAllByPropertyValue<T>(property, value).SingleOrDefault();
        }

        /// <summary>
        /// Gets the table indexes cache for specified entity type.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>table indexes cache for specified entity type.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        private TableIndexes<T> GetTableIndexesCache<T>() where T : class
        {
            object cache;
            if (this.TableIndexesCaches.TryGetValue(typeof(T), out cache))
            {
                return (TableIndexes<T>)cache;
            }
            else
            {
                TableIndexes<T> tableCache = new TableIndexes<T>(this.Repository.All<T>().ToList());
                this.TableIndexesCaches.Add(typeof(T), tableCache);

                return tableCache;
            }
        }

        #endregion
    }
}