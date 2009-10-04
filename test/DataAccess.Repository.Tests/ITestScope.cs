// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestScope.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Empty Scope for internal repository tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Extended;

    /// <summary>
    /// Empty Scope for internal repository tests
    /// </summary>
    public interface ITestScope : IScope
    {
        #region Properties

        /// <summary>
        /// Gets the test value.
        /// </summary>
        /// <value>The test value.</value>
        string TestValue { get; }

        #endregion
    }
}