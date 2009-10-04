// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestScope.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The test scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    /// <summary>
    /// The test scope.
    /// </summary>
    public class TestScope : ITestScope
    {
        #region Implemented Interfaces (Properties)

        #region ITestScope properties

        /// <summary>
        /// Gets or sets TestValue.
        /// </summary>
        public string TestValue { get; set; }

        #endregion

        #endregion
    }
}