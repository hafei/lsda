// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableIndex.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Entity table index
    /// </summary>
    /// <typeparam name="T">
    /// Entity type
    /// </typeparam>
    internal class TableIndex<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableIndex&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tableData">
        /// The table data.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        public TableIndex(IEnumerable<T> tableData, PropertyInfo property)
        {
            this.IndexTableData(tableData, property);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the index data.
        /// </summary>
        /// <value>The index data.</value>
        private Dictionary<object, List<T>> Index { get; set; }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets the entities with the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <value>Entities with the specified value.</value>
        public List<T> this[object value]
        {
            get
            {
                List<T> result;
                if (this.Index.TryGetValue(value, out result))
                {
                    return result;
                }
                else
                {
                    return new List<T>(); // or null ??
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Indexes the table data.
        /// </summary>
        /// <param name="tableData">
        /// The table data.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        private void IndexTableData(IEnumerable<T> tableData, PropertyInfo property)
        {
            this.Index = new Dictionary<object, List<T>>();

            foreach (T entity in tableData)
            {
                object propValue = property.GetValue(entity, null);

                // add nullable support
                List<T> knownValues;
                if (!this.Index.TryGetValue(propValue, out knownValues))
                {
                    knownValues = new List<T>();
                    this.Index[propValue] = knownValues;
                }

                knownValues.Add(entity);
            }
        }

        #endregion
    }
}