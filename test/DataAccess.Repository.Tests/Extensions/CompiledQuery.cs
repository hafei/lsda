// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompiledQuery.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The compiled query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The compiled query.
    /// </summary>
    public class CompiledQuery
    {
        #region Constants and Fields

        /// <summary>
        /// The compiled query type.
        /// </summary>
        public static readonly Type CompiledQueryType;

        /// <summary>
        /// The query infos field.
        /// </summary>
        private static readonly FieldInfo QueryInfosField;

        /// <summary>
        /// The sub queries field.
        /// </summary>
        private static readonly FieldInfo SubQueriesField;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="CompiledQuery"/> class.
        /// </summary>
        static CompiledQuery()
        {
            CompiledQueryType = SqlProvider.SqlProviderType.GetNestedType("CompiledQuery", BindingFlags.NonPublic);

            QueryInfosField = CompiledQueryType.GetField("queryInfos", BindingFlags.NonPublic | BindingFlags.Instance);
            SubQueriesField = CompiledQueryType.GetField("subQueries", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledQuery"/> class.
        /// </summary>
        /// <param name="internalValue">
        /// The internal value.
        /// </param>
        public CompiledQuery(object internalValue)
        {
            if (!CompiledQueryType.IsAssignableFrom(internalValue.GetType()))
            {
                throw new ArgumentException("Wrong object provided.");
            }

            this.InternalValue = internalValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets QueryInfos.
        /// </summary>
        public List<QueryInfo> QueryInfos
        {
            get
            {
                var queryInfos = QueryInfosField.GetValue(this.InternalValue) as IEnumerable;
                if (queryInfos != null)
                {
                    var list = new List<QueryInfo>();

                    foreach (var queryInfo in queryInfos)
                    {
                        list.Add(new QueryInfo(queryInfo));
                    }

                    return list;
                }

                return new List<QueryInfo>();
            }
        }

        /// <summary>
        /// Gets SubQueries.
        /// </summary>
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