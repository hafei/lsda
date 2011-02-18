// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultLifetimeManagerChangingStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The default lifetime manager changing strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// The default lifetime manager changing strategy.
    /// </summary>
    /// <typeparam name="TLifetimeManager">
    /// The type of the default lifetime manager.
    /// </typeparam>
    public class DefaultLifetimeManagerChangingStrategy<TLifetimeManager> : BuilderStrategy
        where TLifetimeManager : LifetimeManager, new()
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
            // if there is no lifetime policy
            if (context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey) == null)
            {
                // then applying default lifetime manager
                // note: not setting InUse = true here, but not a big deal
                var newLifetime = new TLifetimeManager();

                // note: policy will be set in the container itself
                context.PersistentPolicies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
                context.Lifetime.Add(newLifetime);
            }
        }

        #endregion
    }
}