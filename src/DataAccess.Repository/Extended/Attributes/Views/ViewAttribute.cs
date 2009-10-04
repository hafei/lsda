// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The view attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Attributes.Views
{
    using System;

    /// <summary>
    /// The view attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ViewAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAttribute"/> class.
        /// </summary>
        /// <param name="rootType">
        /// The root type.
        /// </param>
        public ViewAttribute(Type rootType)
        {
            this.RootType = rootType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Type.
        /// </summary>
        public Type RootType { get; private set; }

        #endregion
    }
}