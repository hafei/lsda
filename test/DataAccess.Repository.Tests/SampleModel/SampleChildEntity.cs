// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleChildEntity.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample child entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    /// <summary>
    /// The sample child entity.
    /// </summary>
    public class SampleChildEntity
    {
        #region Properties

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

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public SampleParentEntity Parent { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>The parent id.</value>
        public int ParentId { get; set; }

        #endregion
    }
}