// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The custom expression attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The custom expression attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExpressionAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAttribute"/> class.
        /// </summary>
        /// <param name="declaringType">
        /// The declaring Type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public ExpressionAttribute(Type declaringType, string methodName)
            : this(methodName)
        {
            this.DeclaringType = declaringType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        public ExpressionAttribute(string methodName)
        {
            this.MethodName = methodName;
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