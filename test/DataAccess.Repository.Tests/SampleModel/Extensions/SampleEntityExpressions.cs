// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleEntityExpressions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample entity expressions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.SampleModel.Extensions
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// The sample entity expressions.
    /// </summary>
    public static class SampleEntityExpressions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the specified entity's name equals to "Sample".
        /// </summary>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <returns>
        /// True if entity's name equals to "Sample".
        /// </returns>
        public static Expression<Func<SampleEntity, bool>> IsSample(ITestScope scope)
        {
            return e => e.Name == "Sample";
        }

        #endregion
    }
}