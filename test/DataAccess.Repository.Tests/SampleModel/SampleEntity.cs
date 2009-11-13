// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntity.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Sample entity class that has no relations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel
{
    using Extended.Attributes;

    using Interceptors;

    /// <summary>
    /// Sample entity class that has no relations.
    /// </summary>
    [Intercept(typeof(TestOperationInterceptor))]
    public class SampleEntity
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

        #endregion
    }
}