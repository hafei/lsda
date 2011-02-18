// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerResolveSingletonStrategy.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The per resolve singleton strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.EntLib.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// The per resolve singleton strategy.
    /// </summary>
    public class PerResolveSingletonStrategy : BuilderStrategy
    {
        #region Public Methods

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">
        /// Context of the build operation.
        /// </param>
        public override void PostBuildUp(IBuilderContext context)
        {
            // this strategy will execute before LifetimeStrategy during PostBuildUp stage, so all we need to do here is to remove local perresolve added during PreBuildUp
            // this will leave original lifetime manager on top and LifetimeStrategy will successfully provide built object to it
            // note: this method will not be called without PreBuildUp method being called first, so we can just remove local lifetime policy
            context.Policies.Clear<ILifetimePolicy>(context.BuildKey);
        }

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
            // this strategy will execute after LifetimeStrategy, this means the following
            // 1. if there were existing object in lifetime manager, then build up process is already finished so we can't be here
            // 2. current build key will definitely have lifetime manager, because LifetimeStrategy creates transient lifetime manager for all build keys that have no lifetime policy specified

            // the main idea here is the following: no matter what lifetime manager we have here we change it locally to perresolve lifetime manager to allow cyclic dependencies
            // note: not setting InUse = true here, but not a big deal
            context.Policies.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), context.BuildKey);
        }

        #endregion
    }
}