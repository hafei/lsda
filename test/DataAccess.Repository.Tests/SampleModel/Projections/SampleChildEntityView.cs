// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleChildEntityView.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample child entity view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Projections
{
    using Extended.Interceptors.Common.Attributes;

    /// <summary>
    /// The sample child entity view.
    /// </summary>
    [Projection(typeof(SampleChildEntity))]
    public class SampleChildEntityView
    {
        #region Properties

        /// <summary>
        /// Gets or sets EntityName.
        /// </summary>
        [SelectProperty("Name")]
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [SelectProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ParentName.
        /// </summary>
        [SelectProperty("Parent.Name")]
        public string ParentName { get; set; }

        /// <summary>
        /// Gets or sets SuperParentName.
        /// </summary>
        [SelectProperty("Parent.SuperParent.Name")]
        public string SuperParentName { get; set; }

        #endregion
    }
}