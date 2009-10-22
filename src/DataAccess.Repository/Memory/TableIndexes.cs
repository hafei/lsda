// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableIndexes.cs" company="Logic Software">
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
    using System.Reflection;

    /// <summary>
    /// Table Indexes collection for entity type
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    internal class TableIndexes<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableIndexes&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tableData">The table data.</param>
        public TableIndexes(IEnumerable<T> tableData)
        {
            this.TableData = tableData;
            this.Indexes = new Dictionary<PropertyInfo, TableIndex<T>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the indexed data.
        /// </summary>
        /// <value>The indexed data.</value>
        private Dictionary<PropertyInfo, TableIndex<T>> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the table data.
        /// </summary>
        /// <value>The table data.</value>
        private IEnumerable<T> TableData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Indexes the by property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public TableIndex<T> this[PropertyInfo property]
        {
            get
            {
                TableIndex<T> index;
                if (!this.Indexes.TryGetValue(property, out index))
                {
                    index = new TableIndex<T>(this.TableData, property);

                    this.Indexes[property] = index;
                }

                return index;
            }
        }

        #endregion
    }
}