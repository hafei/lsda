// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Marks class as a target for data access interceptor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Attributes
{
    using System;

    /// <summary>
    /// Marks class as a target for data access interceptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public sealed class InterceptAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptAttribute"/> class.
        /// </summary>
        /// <param name="interceptorType">
        /// Type of the interceptor.
        /// </param>
        public InterceptAttribute(Type interceptorType)
        {
            this.InterceptorType = interceptorType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the interceptor.
        /// </summary>
        /// <value>The type of the interceptor.</value>
        public Type InterceptorType { get; private set; }

        #endregion
    }
}