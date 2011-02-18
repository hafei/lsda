// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayResolutionStrategyOverridingUnityExtension.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The array resolution strategy overriding unity extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The array resolution strategy overriding unity extension.
    /// </summary>
    public class ArrayResolutionStrategyOverridingUnityExtension : UnityContainerExtension
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
            // todo: find the way to just replace ArrayResolutionStrategy
            // adding SameContextArrayResolutionStrategy before ArrayResolutionStrategy
            this.Context.Strategies.AddNew<SameContextArrayResolutionStrategy>(UnityBuildStage.PreCreation);
        }

        #endregion
    }
}