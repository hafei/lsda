// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Attribute for expression fields support
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Attributes.Views
{
    using System;

    /// <summary>
    /// The map attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ExpressionAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The path of member.
        /// </param>
        public ExpressionAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Method Name
        /// </summary>
        public string MethodName { get; private set; }

        #endregion
    }
}
