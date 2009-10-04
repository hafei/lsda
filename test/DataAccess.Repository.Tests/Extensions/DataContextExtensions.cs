// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The data context extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests.Extensions
{
    using System.Data.Linq;
    using System.Reflection;

    /// <summary>
    /// The data context extensions.
    /// </summary>
    public static class DataContextExtensions
    {
        #region Constants and Fields

        /// <summary>
        /// The provider property.
        /// </summary>
        private static readonly PropertyInfo ProviderProperty = typeof(DataContext).GetProperty("Provider", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        #region Public Methods

        /// <summary>
        /// The provider.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// SqlProvider wrapper.
        /// </returns>
        public static SqlProvider Provider(this DataContext context)
        {
            var provider = ProviderProperty.GetValue(context, new object[0]);

            return new SqlProvider(provider);
        }

        #endregion
    }
}