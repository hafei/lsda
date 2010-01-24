// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptVisitAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Attributes
{
    using System;

    /// <summary>
    /// Marks a method as a target of data interceptor visit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class InterceptVisitAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptVisitAttribute"/> class.
        /// </summary>
        /// <param name="interceptorType">
        /// Type of the interceptor.
        /// </param>
        public InterceptVisitAttribute(Type interceptorType)
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