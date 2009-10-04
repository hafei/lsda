// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryRepositoryMethodEventArgs.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System;

    /// <summary>
    /// Base class for memory repository method event arguments
    /// </summary>
    public class MemoryRepositoryMethodEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepositoryMethodEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public MemoryRepositoryMethodEventArgs(object entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public object Entity { get; private set; }
    }
}