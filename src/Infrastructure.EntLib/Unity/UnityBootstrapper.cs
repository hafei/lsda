// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityBootstrapper.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The unity bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    /// <summary>
    /// The unity bootstrapper.
    /// </summary>
    public static class UnityBootstrapper
    {
        #region Public Methods

        /// <summary>
        /// Bootstraps new IUnityContainer instance.
        /// </summary>
        /// <returns>
        /// New bootstrapped IUnityContainer instance.
        /// </returns>
        public static IUnityContainer Bootstrap()
        {
            // container configuration is read from "unity" configuration section
            return new UnityContainer()
                .LoadConfiguration()
                .AddNewExtension<ArrayResolutionStrategyOverridingUnityExtension>();
        }

        #endregion
    }
}