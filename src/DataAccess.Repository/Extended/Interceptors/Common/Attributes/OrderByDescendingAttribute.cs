// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderByDescendingAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The order by descending attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The order by descending attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class OrderByDescendingAttribute : ProjectionMemberAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByDescendingAttribute"/> class.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public OrderByDescendingAttribute(int order)
        {
            this.Order = order;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; private set; }

        #endregion
    }
}
