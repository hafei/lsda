// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultLifetimeManagerChangingExtension.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The default lifetime manager changing extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The default lifetime manager changing extension.
    /// </summary>
    /// <typeparam name="TLifetimeManager">
    /// The type of the default lifetime manager.
    /// </typeparam>
    public class DefaultLifetimeManagerChangingExtension<TLifetimeManager> : UnityContainerExtension
        where TLifetimeManager : LifetimeManager, new()
    {
        #region Methods

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        /// install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            // adding DefaultLifetimeManagerChangingStrategy before LifetimeStrategy
            this.Context.Strategies.AddNew<DefaultLifetimeManagerChangingStrategy<TLifetimeManager>>(UnityBuildStage.TypeMapping);
        }

        #endregion
    }
}