// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassG1.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Test Guid-identified class
    /// </summary>
    public class ClassG1
    {
        #region Properties

        /// <summary>
        /// Gets or sets the classG2S.
        /// </summary>
        /// <value>The classG2S.</value>
        public List<ClassG2> ClassG2s { get; set; }

        /// <summary>
        /// Gets or sets the row id.
        /// </summary>
        /// <value>The row id.</value>
        public Guid? RowId { get; set; }

        #endregion
    }
}