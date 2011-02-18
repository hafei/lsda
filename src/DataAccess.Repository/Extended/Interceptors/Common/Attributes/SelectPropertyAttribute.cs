// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectPropertyAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The select property attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The select property attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SelectPropertyAttribute : ProjectionMemberAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPropertyAttribute"/> class.
        /// </summary>
        public SelectPropertyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPropertyAttribute"/> class.
        /// </summary>
        /// <param name="path">
        /// The path of member.
        /// </param>
        public SelectPropertyAttribute(string path)
        {
            this.Path = path;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets PropertyPath of member to select.
        /// </summary>
        public string Path { get; private set; }

        #endregion
    }
}
