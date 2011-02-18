// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryInfo.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The query info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extensions.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// The query info.
    /// </summary>
    public class QueryInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The query info type.
        /// </summary>
        public static readonly Type QueryInfoType;

        /// <summary>
        /// The command text property.
        /// </summary>
        private static readonly PropertyInfo CommandTextProperty;

        /// <summary>
        /// The result shape property.
        /// </summary>
        private static readonly PropertyInfo ResultShapeProperty;

        /// <summary>
        /// The result type property.
        /// </summary>
        private static readonly PropertyInfo ResultTypeProperty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="QueryInfo"/> class. 
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static ctor is ok here too.")]
        static QueryInfo()
        {
            QueryInfoType = SqlProvider.SqlProviderType.GetNestedType("QueryInfo", BindingFlags.NonPublic);

            CommandTextProperty = QueryInfoType.GetProperty("CommandText", BindingFlags.NonPublic | BindingFlags.Instance);
            ResultTypeProperty = QueryInfoType.GetProperty("ResultType", BindingFlags.NonPublic | BindingFlags.Instance);
            ResultShapeProperty = QueryInfoType.GetProperty("ResultShape", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryInfo"/> class.
        /// </summary>
        /// <param name="internalValue">
        /// The internal value.
        /// </param>
        public QueryInfo(object internalValue)
        {
            if (internalValue == null)
            {
                throw new ArgumentNullException("internalValue");
            }

            if (!QueryInfoType.IsAssignableFrom(internalValue.GetType()))
            {
                throw new ArgumentException("Wrong object provided.");
            }

            this.InternalValue = internalValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets CommandText.
        /// </summary>
        public string CommandText
        {
            get
            {
                var commandText = CommandTextProperty.GetValue(this.InternalValue, new object[0]);

                return (string) commandText;
            }
        }

        /// <summary>
        /// Gets ResultShape.
        /// </summary>
        public string ResultShape
        {
            get
            {
                var resultShape = ResultShapeProperty.GetValue(this.InternalValue, new object[0]);

                return Enum.GetName(resultShape.GetType(), resultShape);
            }
        }

        /// <summary>
        /// Gets ResultType.
        /// </summary>
        public Type ResultType
        {
            get
            {
                var resultType = ResultTypeProperty.GetValue(this.InternalValue, new object[0]);

                return (Type) resultType;
            }
        }

        /// <summary>
        /// Gets or sets InternalValue.
        /// </summary>
        private object InternalValue { get; set; }

        #endregion
    }
}