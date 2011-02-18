// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandWithExpressionAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The expand with expression attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The expand with expression attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExpandWithExpressionAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandWithExpressionAttribute"/> class.
        /// </summary>
        /// <param name="declaringType">
        /// The declaring Type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public ExpandWithExpressionAttribute(Type declaringType, string methodName)
        {
            this.DeclaringType = declaringType;
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandWithExpressionAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public ExpandWithExpressionAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandWithExpressionAttribute"/> class.
        /// </summary>
        /// <param name="declaringType">
        /// The declaring type.
        /// </param>
        public ExpandWithExpressionAttribute(Type declaringType)
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