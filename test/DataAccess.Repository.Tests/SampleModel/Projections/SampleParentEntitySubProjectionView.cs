// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleParentEntitySubProjectionView.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample parent entity sub projection view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Projections
{
    using System.Collections.Generic;

    using Extended.Interceptors.Common.Attributes;

    /// <summary>
    /// The sample parent entity sub projection view.
    /// </summary>
    [Projection(typeof(SampleParentEntity))]
    public class SampleParentEntitySubProjectionView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the children views.
        /// </summary>
        /// <value>The children views.</value>
        /// <remarks>Note element type, this will be sub-projection.</remarks>
        [Property("Children")]
        public List<SampleChildEntityView> ChildrenViews { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the entity.</value>
        [Property]
        public string Name { get; set; }

        #endregion
    }
}