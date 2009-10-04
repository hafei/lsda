// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The map attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Attributes.Views
{
    using System;

    /// <summary>
    /// The map attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="path">
        /// The path of member.
        /// </param>
        public PropertyAttribute(string path)
        {
            this.Path = path;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets PropertyPath of member.
        /// </summary>
        public string Path { get; private set; }

        #endregion
    }
}