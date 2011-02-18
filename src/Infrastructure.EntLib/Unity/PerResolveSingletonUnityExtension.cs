// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerResolveSingletonUnityExtension.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The per resolve singleton unity extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The per resolve singleton unity extension.
    /// </summary>
    public class PerResolveSingletonUnityExtension : UnityContainerExtension
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
            // adding DefaultLifetimeManagerChangingStrategy after LifetimeStrategy (before in PostBuildUp stage)
            this.Context.Strategies.AddNew<PerResolveSingletonStrategy>(UnityBuildStage.Lifetime);
        }

        #endregion
    }
}