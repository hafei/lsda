// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransientToContainerSingletonLifetimeChangingUnityExtension.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The transient to container singleton lifetime changing unity extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The transient to container singleton lifetime changing unity extension.
    /// </summary>
    public class TransientToContainerSingletonLifetimeChangingUnityExtension : UnityContainerExtension
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
            // adding DefaultLifetimeManagerChangingStrategy after LifetimeStrategy
            this.Context.Strategies.AddNew<TransientToContainerSingletonLifetimeChangingStrategy>(UnityBuildStage.Lifetime);
        }

        #endregion
    }
}