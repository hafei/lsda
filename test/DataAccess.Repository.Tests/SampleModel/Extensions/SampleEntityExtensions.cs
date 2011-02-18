// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntityExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample entity extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Extensions
{
    using System;

    using Extended.Attributes;
    using Extended.Interceptors.Common;
    using Extended.Interceptors.Common.Attributes;

    /// <summary>
    /// The sample entity extensions.
    /// </summary>
    [InterceptVisit(typeof(ExpressionExpanderQueryInterceptor))]
    public static class SampleEntityExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the specified entity is sample.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified entity is sample; otherwise, <c>false</c>.
        /// </returns>
        [ExpandWithExpression(typeof(SampleEntityExpressions))]
        public static bool IsSample(this SampleEntity entity)
        {
            throw new InvalidOperationException("Entity extensions should be used in queries only.");
        }

        #endregion
    }
}