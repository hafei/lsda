// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderByAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The order by attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The order by attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class OrderByAttribute : ProjectionMemberAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByAttribute"/> class.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public OrderByAttribute(int order)
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
