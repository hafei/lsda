// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionAttribute.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The projection attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common.Attributes
{
    using System;

    /// <summary>
    /// The projection attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProjectionAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionAttribute"/> class.
        /// </summary>
        /// <param name="rootType">
        /// The root entity type of the projection.
        /// </param>
        public ProjectionAttribute(Type rootType)
        {
            this.RootType = rootType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the root entity type of the projection.
        /// </summary>
        public Type RootType { get; private set; }

        #endregion
    }
}