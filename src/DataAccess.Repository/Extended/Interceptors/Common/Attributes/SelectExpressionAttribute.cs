// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectExpressionAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The select expression attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The select expression attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SelectExpressionAttribute : ProjectionMemberAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpressionAttribute"/> class.
        /// </summary>
        /// <param name="declaringType">
        /// The declaring Type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public SelectExpressionAttribute(Type declaringType, string methodName)
        {
            this.DeclaringType = declaringType;
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpressionAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public SelectExpressionAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpressionAttribute"/> class.
        /// </summary>
        /// <param name="declaringType">
        /// The declaring type.
        /// </param>
        public SelectExpressionAttribute(Type declaringType)
        {
            this.DeclaringType = declaringType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the method declaring type.
        /// </summary>
        /// <value>
        /// The method declaring type.
        /// </value>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName { get; private set; }

        #endregion
    }
}
