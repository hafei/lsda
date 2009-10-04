// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryRepositoryStatusEventArgs.cs" company="Logic Software">
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
    /// Base class for memory repository status event arguments
    /// </summary>
    public class MemoryRepositoryStatusEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepositoryStatusEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="oldValue">The old value.</param>
        public MemoryRepositoryStatusEventArgs(object result, object oldValue)
        {
            this.Result = result;
            this.OldValue = oldValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue { get; set; }

        #endregion
    }
}