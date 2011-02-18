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
    using System.Configuration;

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
            IUnityContainer container = new UnityContainer();

            UnityConfigurationSection configuration = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
            configuration.Configure(container);

            return container;
        }

        #endregion
    }
}