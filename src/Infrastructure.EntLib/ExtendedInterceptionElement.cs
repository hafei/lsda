// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedInterceptionElement.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The extended interception element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib
{
    using System;
    using System.Configuration;
    using System.Globalization;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// The extended interception element.
    /// </summary>
    public class ExtendedInterceptionElement : UnityContainerExtensionConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets the interceptor.
        /// </summary>
        /// <value>The interceptor.</value>
        [ConfigurationProperty("interceptor", IsRequired = false, DefaultValue = "")]
        public string Interceptor
        {
            get
            {
                return (string) this["interceptor"];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <param name="container">
        /// The <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/> to configure.
        /// </param>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.
        /// </remarks>
        public override void Configure(IUnityContainer container)
        {
            base.Configure(container);

            ExtendedInterception interception = new ExtendedInterception();

            var type = System.Type.GetType(this.Interceptor, false, false);
            if (type == null || !typeof(IInterceptor).IsAssignableFrom(type))
            {
                throw new ConfigurationErrorsException(String.Format(CultureInfo.InvariantCulture, "The '{0}' is not a valid Interceptor.", this.Interceptor));
            }

            interception.Interceptor = (IInterceptor) Activator.CreateInstance(type);

            container.AddExtension(interception);
        }

        #endregion
    }
}