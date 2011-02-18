// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransientToContainerSingletonLifetimeChangingStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The transient to container singleton lifetime changing strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// The transient to container singleton lifetime changing strategy.
    /// </summary>
    public class TransientToContainerSingletonLifetimeChangingStrategy : BuilderStrategy
    {
        #region Public Methods

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">
        /// Context of the build operation.
        /// </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            // this strategy will execute after LifetimeStrategy, this means that current build key will definitely have lifetime manager, because LifetimeStrategy creates transient lifetime manager for all build keys that have no lifetime policy specified
            // note: we are interested only in persistent lifetimes only, because i.e. perresovle lifetime implementation creates local lifetime managers that will hide real persistent ones
            var originalLifetime = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey);

            if (originalLifetime is TransientLifetimeManager)
            {
                // then applying hierarchical lifetime manager to allow every child container to have its own instance of object being built
                // note: not setting InUse = true here, but not a big deal
                var newLifetime = new HierarchicalLifetimeManager();

                // note: policy will be set in the container itself (but during the current build it may be hidden by local lifetime manager)
                context.PersistentPolicies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);

                // note: it is crucial to add new lifetime manager to container's lifetime to allow proper disposal of created objects
                context.Lifetime.Add(newLifetime);
            }
        }

        #endregion
    }
}