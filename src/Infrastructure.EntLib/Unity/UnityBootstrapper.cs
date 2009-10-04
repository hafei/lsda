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

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.PolicyInjection.Configuration;
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
            configuration.Containers.Default.Configure(container);

            // configuring Unity container for policy injection
            IConfigurationSource configSource = ConfigurationSourceFactory.Create();
            PolicyInjectionSettings section = (PolicyInjectionSettings) configSource.GetSection("policyInjection");
            if (section != null)
            {
                section.ConfigureContainer(container, configSource);
            }

            return container;
        }

        #endregion
    }
}