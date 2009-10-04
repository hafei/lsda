// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassG2.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using System;

    /// <summary>
    /// Test Guid-identified class
    /// </summary>
    public class ClassG2
    {
        #region Properties

        /// <summary>
        /// Gets or sets the class g1.
        /// </summary>
        /// <value>The class g1.</value>
        public ClassG1 ClassG1 { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The G2 entity id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the row id.
        /// </summary>
        /// <value>The row id.</value>
        public Guid RowIdRef { get; set; }

        #endregion
    }
}