// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompiledSubQuery.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The compiled sub query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extensions.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// The compiled sub query.
    /// </summary>
    public class CompiledSubQuery
    {
        #region Constants and Fields

        /// <summary>
        /// The compiled sub query type.
        /// </summary>
        public static readonly Type CompiledSubQueryType;

        /// <summary>
        /// The query info field.
        /// </summary>
        private static readonly FieldInfo QueryInfoField;

        /// <summary>
        /// The sub queries field.
        /// </summary>
        private static readonly FieldInfo SubQueriesField;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="CompiledSubQuery"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static ctor is ok here too.")]
        static CompiledSubQuery()
        {
            CompiledSubQueryType = SqlProvider.SqlProviderType.GetNestedType("CompiledSubQuery", BindingFlags.NonPublic);

            SubQueriesField = CompiledSubQueryType.GetField("subQueries", BindingFlags.NonPublic | BindingFlags.Instance);
            QueryInfoField = CompiledSubQueryType.GetField("queryInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledSubQuery"/> class.
        /// </summary>
        /// <param name="internalValue">
        /// The internal value.
        /// </param>
        public CompiledSubQuery(object internalValue)
        {
            if (!CompiledSubQueryType.IsAssignableFrom(internalValue.GetType()))
            {
                throw new ArgumentException("Wrong object provided.");
            }

            this.InternalValue = internalValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets QueryInfo.
        /// </summary>
        public QueryInfo QueryInfo
        {
            get
            {
                var queryInfo = QueryInfoField.GetValue(this.InternalValue);

                return new QueryInfo(queryInfo);
            }
        }

        /// <summary>
        /// Gets SubQueries.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "No need in special rules here.")]
        public List<CompiledSubQuery> SubQueries
        {
            get
            {
                var subQueries = SubQueriesField.GetValue(this.InternalValue) as IEnumerable;
                if (subQueries != null)
                {
                    var list = new List<CompiledSubQuery>();

                    foreach (var subQuery in subQueries)
                    {
                        list.Add(new CompiledSubQuery(subQuery));
                    }

                    return list;
                }

                return new List<CompiledSubQuery>();
            }
        }

        /// <summary>
        /// Gets or sets InternalValue.
        /// </summary>
        private object InternalValue { get; set; }

        #endregion
    }
}