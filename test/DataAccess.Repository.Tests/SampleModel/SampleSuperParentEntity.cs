// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleSuperParentEntity.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample super parent entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The sample super parent entity.
    /// </summary>
    public class SampleSuperParentEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children entities.</value>
        public List<SampleParentEntity> Children { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id of the entity.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string Name { get; set; }

        #endregion
    }
}