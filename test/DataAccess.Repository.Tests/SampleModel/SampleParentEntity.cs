// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleParentEntity.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample parent entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The sample parent entity.
    /// </summary>
    public class SampleParentEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children entities.</value>
        public List<SampleChildEntity> Children { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id ot the entity.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the super parent.
        /// </summary>
        /// <value>The super parent.</value>
        public SampleSuperParentEntity SuperParent { get; set; }

        /// <summary>
        /// Gets or sets the super parent id.
        /// </summary>
        /// <value>The super parent id.</value>
        public int SuperParentId { get; set; }

        #endregion
    }
}