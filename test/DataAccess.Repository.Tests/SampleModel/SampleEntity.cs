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
    using System;

    using Extended.Attributes;
    using Extended.Interceptors.Common;
    using Extended.Interceptors.Common.Attributes;

    using Extensions;

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

        #region Public Methods

        /// <summary>
        /// Determines whether this instance is sample.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is sample; otherwise, <c>false</c>.
        /// </returns>
        [Expression(typeof(SampleEntityExpressions), "IsSample")]
        [InterceptVisit(typeof(ExpressionExpanderQueryInterceptor))]
        public bool IsSampleInstance()
        {
            throw new InvalidOperationException("Entity extensions should be used in queries only.");
        }

        #endregion
    }
}